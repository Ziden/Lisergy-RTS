using Game;
using Game.Battler;
using Game.Dungeon;
using Game.Events;
using Game.Events.ServerEvents;
using Game.Network.ServerPackets;
using Game.Party;
using Game.Scheduler;
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
            GameScheduler.Clear();
        }

        [TearDown]
        public void Tear()
        {
            _game.ClearEventListeners();
        }

        [Test]
        public void TestPartyStatusUpdatedAfterBattle()
        {
            var playerCastleTile = _player.Buildings.First().Tile;
            var party = _player.GetParty(0);
            party.Tile = _game.World.GetTile(0, 0);
            var enemy = new DungeonEntity();
            enemy.Tile = _game.World.GetTile(1, 1);
            enemy.BattleGroupLogic.AddUnit(new Unit(0));

            var battleID = Guid.NewGuid();
            _game.NetworkEvents.Call(new BattleStartPacket(battleID, party, enemy));

            var battle = _game.BattleService.GetBattle(battleID);
            battle.Task.Execute();

            var statusUpdates = _player.ReceivedEventsOfType<PartyStatusUpdatePacket>();
            Assert.AreEqual(1, statusUpdates.Count);
        }

        [Test]
        public void TestPartyReplaceUnits()
        {
            var unit1 = new Unit(0);
            var unit2 = new Unit(1);
            var unit3 = new Unit(2);

            var party = new PartyEntity(_player);
            party.BattleGroupLogic.AddUnit(unit1);
            party.BattleGroupLogic.AddUnit(unit2);

            party.BattleGroupLogic.ReplaceUnit(unit1, unit3);

            Assert.AreEqual(2, party.BattleGroupLogic.GetUnits().Count());
            Assert.IsTrue(party.BattleGroupLogic.GetUnits().Contains(unit3));
            Assert.IsTrue(party.BattleGroupLogic.GetUnits().Contains(unit2));
            Assert.IsFalse(party.BattleGroupLogic.GetUnits().Contains(unit1));
        }

        [Test]
        public void TestReplaceAtIndex()
        {
            var unit1 = new Unit(0);
            var unit2 = new Unit(0);
            var unit3 = new Unit(2);

            var party = new PartyEntity(_player);
            party.BattleGroupLogic.AddUnit(unit1);
            party.BattleGroupLogic.AddUnit(unit2);

            party.BattleGroupLogic.ReplaceUnit(unit2, unit3, 1);

            Assert.AreEqual(2, party.BattleGroupLogic.GetUnits().Count());
        }

        [Test]
        public void TestPartyUpdateUpdates()
        {
            var unit0 = new Unit(0);
            var unit1 = new Unit(1);
            var unit2 = new Unit(2);

            var party = new PartyEntity(_player);
            party.BattleGroupLogic.AddUnit(unit0);
            party.BattleGroupLogic.AddUnit(unit1);
            party.BattleGroupLogic.AddUnit(unit0);
            party.BattleGroupLogic.AddUnit(unit1);

            var newUnits = new List<Unit>() { unit1, unit2, unit0 };

            party.BattleGroupLogic.UpdateUnits(newUnits);
            var units = party.BattleGroupLogic.GetUnits().ToList();

            Assert.AreEqual(3, party.BattleGroupLogic.GetUnits().Count());
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
            Assert.IsTrue(unitsComponent.FrontLine().SequenceEqual(party.BattleGroupLogic.GetUnits()));
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