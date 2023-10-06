using Game;
using Game.DataTypes;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Network.ServerPackets;
using Game.Scheduler;
using Game.Systems.Battler;
using Game.Systems.Dungeon;
using Game.Systems.Party;
using NUnit.Framework;
using ServerTests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class TestParty
    {
        private TestGame _game;
        private TestServerPlayer _player;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
        }

        [Test]
        public void TestPartyStatusUpdatedAfterBattle()
        {
            var playerCastleTile = _player.Data.Buildings.First().Tile;
            var party = _player.GetParty(0);
            _game.Systems.Map.GetEntityLogic(party).SetPosition(_game.World.GetTile(0, 0));

            var enemy = _game.Entities.CreateEntity<DungeonEntity>(null);
            _game.Systems.Map.GetEntityLogic(enemy).SetPosition(_game.World.GetTile(1, 1));

            enemy.Get<BattleGroupComponent>().Units.Add(new Unit(_game.Specs.Units[0]));

            var battleID = Guid.NewGuid();
            _game.Network.IncomingPackets.Call(new BattleStartPacket(battleID, party, enemy));

            _player.ReceivedEvents.Clear();
            DeltaTracker.Clear();

            var battle = _game.BattleService.GetBattle(battleID);
            battle.Task.Tick();

            var statusUpdates = _player.ReceivedEventsOfType<EntityUpdatePacket>().Where(p => p.Entity.EntityId == party.EntityId);
            Assert.AreEqual(1, statusUpdates.Count());
        }


        [Test]
        public void TestPartyUnitsInitialHP()
        {
            var team = _player.GetParty(0).Get<BattleGroupComponent>().Units;

            var unit = team.First();

            var mhp = unit.MaxHP;

            Assert.That(unit.HP == unit.MaxHP && unit.HP > 0);
        }


        [Test]
        public void TestBattleIDResetsAfterBattle()
        {
            var party = _player.GetParty(0);
            _game.Logic.Map(party).SetPosition(_game.World.GetTile(0, 0));

            var enemy = _game.Entities.CreateEntity<DungeonEntity>(null);
            _game.Logic.Map(enemy).SetPosition(_game.World.GetTile(0, 0));

            enemy.Get<BattleGroupComponent>().Units.Add(new Unit(_game.Specs.Units[0]));

            var battleID = Guid.NewGuid();
            _game.Network.IncomingPackets.Call(new BattleStartPacket(battleID, party, enemy));

            var battle = _game.BattleService.GetBattle(battleID);
            battle.Task.Tick();

            Assert.IsTrue(party.Get<BattleGroupComponent>().BattleID == GameId.ZERO);
            Assert.IsTrue(battle.IsOver);
           
        }

        [Test]
        public void TestPartyOwner()
        {
            var party = _player.GetParty(0);

            Assert.AreEqual(party.OwnerID, _player.EntityId);
        }

        [Test]
        public void TestPartyReplaceUnits()
        {
            var unit1 = new Unit(_game.Specs.Units[0]);
            var unit2 = new Unit(_game.Specs.Units[1]);
            var unit3 = new Unit(_game.Specs.Units[2]);

            var party = _game.Entities.CreateEntity<PartyEntity>(_player);
            var logic = _game.EntityLogic(party).BattleGroup;
            logic.AddUnit(unit1);
            logic.AddUnit(unit2);

            logic.ReplaceUnit(unit1, unit3);
   
            Assert.AreEqual(2, logic.GetUnits().Count());
            Assert.IsTrue(logic.GetUnits().Contains(unit3));
            Assert.IsTrue(logic.GetUnits().Contains(unit2));
            Assert.IsFalse(logic.GetUnits().Contains(unit1));
        }

        [Test]
        public void TestReplaceAtIndexLogic()
        {
            var unit1 = new Unit(_game.Specs.Units[0]);
            var unit2 = new Unit(_game.Specs.Units[0]);
            var unit3 = new Unit(_game.Specs.Units[2]);

            var party = _game.Entities.CreateEntity<PartyEntity>(_player);
            _game.EntityLogic(party).BattleGroup.AddUnit(unit1);
            _game.EntityLogic(party).BattleGroup.AddUnit(unit2);
            _game.EntityLogic(party).BattleGroup.ReplaceUnit(unit2, unit3, 1);

            Assert.AreEqual(2, _game.EntityLogic(party).BattleGroup.GetUnits().Count());
        }

        [Test]
        public void TestPartyUpdateUpdates()
        {
            var unit0 = new Unit(_game.Specs.Units[0]);
            var unit1 = new Unit(_game.Specs.Units[1]);
            var unit2 = new Unit(_game.Specs.Units[2]);

            var party = _game.Entities.CreateEntity<PartyEntity>(_player);
            var logic = _game.EntityLogic(party).BattleGroup;
            logic.AddUnit(unit0);
            logic.AddUnit(unit1);
            logic.AddUnit(unit0);
            logic.AddUnit(unit1);

            var newUnits = new List<Unit>() { unit1, unit2, unit0 };

            logic.UpdateUnits(newUnits);
            var units = logic.GetUnits().ToList();

            Assert.AreEqual(3, logic.GetUnits().Count());
            Assert.AreEqual(units[0], unit1);
            Assert.AreEqual(units[1], unit2);
            Assert.AreEqual(units[2], unit0);
 
        }

        [Test]
        public void TestPartyBattleUnitsSerialized()
        {
            var party = _player.GetParty(0);

            var update = party.GetUpdatePacket(_player);

            var serialize = Serialization.FromEventRaw(update);
            var deserialize = Serialization.ToEvent<EntityUpdatePacket>(serialize);

            var unitsComponent = (BattleGroupComponent)deserialize.SyncedComponents.FirstOrDefault(c => c.GetType() == typeof(BattleGroupComponent));

            Assert.IsTrue(unitsComponent != null);
            Assert.IsTrue(unitsComponent.Units.SequenceEqual(party.Get<BattleGroupComponent>().Units));
        }

        [Test]
        public void TestPartyDoesNotReceiveDestroyPacket()
        {
            var party = _player.GetParty(0);
            party.Components.Get<PartyComponent>().PartyIndex = 2;

            var update = party.GetUpdatePacket(_player);

            var serialize = Serialization.FromEventRaw(update);
            var deserialize = Serialization.ToEvent<EntityUpdatePacket>(serialize);

            Assert.IsTrue(deserialize.SyncedComponents.Any(c => c is PartyComponent));
            Assert.IsTrue(((PartyComponent)deserialize.SyncedComponents.First(c => c is PartyComponent)).PartyIndex == 2);

        }
    }
}