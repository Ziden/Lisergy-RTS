using Game.Battle;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Network.ClientPackets;
using Game.Network.ServerPackets;
using Game.Scheduler;
using Game.Systems.Battler;
using Game.Systems.Dungeon;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Systems.Tile;
using Game.World;
using NUnit.Framework;
using ServerTests;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class TestDungeon
    {
        private TestGame _game;
        private List<Position> _path;
        private TestServerPlayer _player;
        private PartyEntity _party;
        private DungeonEntity _dungeon;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _path = new List<Position>();
            _party = _player.GetParty(0);
            _dungeon = _game.Entities.CreateEntity<DungeonEntity>(null);
            _dungeon.BuildFromSpec(_game.Specs.Dungeons[0]);
            Assert.That(_dungeon.Get<BattleGroupComponent>().Units.Count == 1);
            _game.Logic.Map(_dungeon).SetPosition(_game.World.GetTile(8, 8));
            GameScheduler.Clear();
        }

        [TearDown]
        public void Tear()
        {
            _game.ClearEventListeners();
        }

        [Test]
        public void TestPassiveMoveTowardsDungeon()
        {
            var playerCastleTile = _player.Data.Buildings.First().Tile;
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
            var party = _player.GetParty(0);

            // place the dungeon
            dungeonTile.Components.Get<TileHabitants>().Building = _dungeon;

            // send intent to move player to the party
            _player.SendMoveRequest(party, dungeonTile, MovementIntent.Defensive);
            var course = _player.GetParty(0).Course;

            // Complete the move intent
            GameScheduler.ForceComplete(course);

            Assert.AreEqual(dungeonTile, party.Tile);
            Assert.IsTrue(party.Components.Get<BattleGroupComponent>().BattleID.IsZero());
            Assert.AreEqual(0, _player.Data.BattleHeaders.Count());
        }

        [Test]
        public void TestAgressiveMoveOnDungeon()
        {
            var party = _player.GetParty(0);
            var partyTile = party.Tile;
            var dungeonTile = partyTile.GetNeighbor(Direction.EAST);

            _game.Logic.Map(_dungeon).SetPosition(dungeonTile);
  
            party.Components.Get<BattleGroupComponent>().Units.First().Atk = 255;

            _player.SendMoveRequest(party, dungeonTile, MovementIntent.Offensive);
            var course = party.Course;

            Assert.AreEqual(0, _player.Data.BattleHeaders.Count());

            course.Tick();
            course.Tick();

            var battle = _game.BattleService.GetBattle(party.Components.Get<BattleGroupComponent>().BattleID);
            battle.Task.Tick();

            Assert.AreEqual(dungeonTile, party.Tile);
            Assert.AreEqual(1, _player.Data.BattleHeaders.Count);
        }


        [Test]
        public void TestDungeonComponentsRegisterBuilding()
        {
            var playerCastleTile = _player.Data.Buildings.First().Tile;
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);

            _game.Logic.Map(_dungeon).SetPosition(dungeonTile);

            var tileBuilding = dungeonTile.Components.Get<TileHabitants>().Building;
            Assert.That(tileBuilding.EntityId == _dungeon.EntityId);
            Assert.That(tileBuilding.Components.Get<BattleGroupComponent>().Units.Count > 0);
        }

        [Test]
        public void TestDungeonSpawnsWithMaxHPUnits()
        {
            var team = _dungeon.Components.Get<BattleGroupComponent>().Units;

            var unit = team.First();

            Assert.That(unit.HP == unit.MaxHP && unit.HP > 0);
        }

        [Test]
        public void TestDungeonRemovedWhenComplete()
        {
            var playerCastleTile = _player.Data.Buildings.First().Tile;
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
            var party = _player.GetParty(0);

            party.Components.Get<BattleGroupComponent>().Units.First().Atk = 25; // make sure it wins !
            _game.Logic.Map(_dungeon).SetPosition(dungeonTile);

            _player.SendMoveRequest(party, dungeonTile, MovementIntent.Offensive);
            _player.GetParty(0).Course.Tick();

            var battle = _game.BattleService.GetBattle(party.Components.Get<BattleGroupComponent>().BattleID);
            battle.Task.Tick();

            Assert.IsTrue(_dungeon.Tile == null);
            Assert.IsTrue(_game.Logic.BattleGroup(_dungeon).IsDestroyed);
            Assert.AreEqual(_dungeon.Tile, null);
            Assert.AreEqual(dungeonTile.Components.Get<TileHabitants>().Building, null);
            Assert.AreEqual(_player.ReceivedEventsOfType<EntityDestroyPacket>().Count, 1);
        }

        [Test]
        public void TestRevealingDungeonReceivesPacket()
        {
            _dungeon = _game.Entities.CreateEntity<DungeonEntity>(null);
            _dungeon.Get<BattleGroupComponent>().Units.Add(new Unit(_game.Specs.Units[1]));
            _game.Logic.Map(_dungeon).SetPosition(_game.World.GetTile(_party.Tile.X + _party.GetLineOfSight() + 1, _party.Tile.Y));

            DeltaTracker.Clear();

            var seenClose = _dungeon.Tile.GetNeighbor(Direction.WEST);

            Assert.That(seenClose.EntitiesViewing.Contains(_party));
            Assert.That(!_dungeon.Tile.EntitiesViewing.Contains(_party));

            _player.ReceivedEvents.Clear();
            _game.Logic.Map(_party).SetPosition(_party.Tile.GetNeighbor(Direction.EAST));
            DeltaTracker.SendDeltaPackets(_player);

            Assert.That(_player.ReceivedEventsOfType<EntityUpdatePacket>().Any(p => p.Entity.GetType() == typeof(DungeonEntity)));
            Assert.That(_dungeon.Tile.EntitiesViewing.Contains(_party));
        }

        [Test]
        public void TestDungeonBattleTeamHasUnits()
        {
            _dungeon = _game.Entities.CreateEntity<DungeonEntity>(null);
            _dungeon.Get<BattleGroupComponent>().Units.Add(new Unit(_game.Specs.Units[1]));

            var battleTeam = new BattleTeam(_dungeon, _dungeon.Get<BattleGroupComponent>().Units.ToArray());
            Assert.AreNotEqual(battleTeam.Units.Length, 0);
        }

        [Test]
        public void TestReceivingDungeonWhenLogin()
        {
            var joinEvent = new JoinWorldPacket();
            var clientPlayer = new TestServerPlayer(_game);
            _player.ReceivedEvents.Clear();

            var dg = _game.Entities.CreateEntity<DungeonEntity>(null);
            _game.Logic.Map(dg).SetPosition(_game.World.GetTile(4, 2));

            _game.HandleClientEvent(clientPlayer, joinEvent);

            var entityVisibleEvents = clientPlayer.ReceivedEventsOfType<EntityUpdatePacket>();

            Assert.That(entityVisibleEvents.Any(p => p.Entity.GetType() == typeof(DungeonEntity)));
            Assert.AreEqual(3, entityVisibleEvents.Count, "Initial Party & Building should be visible");
        }

        [Test]
        public void TestPartyDefeatHealsDungeon()
        {
            var playerCastleTile = _player.Data.Buildings.First().Tile;
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
            var party = _player.GetParty(0);
            party.Get<BattleGroupComponent>().Units.First().HP = 1; // make sure it looses !
            party.Get<BattleGroupComponent>().Units.First().Atk = 0; // make sure it looses !
            dungeonTile.Components.Get<TileHabitants>().Building = _dungeon;

            _player.SendMoveRequest(_player.GetParty(0), dungeonTile, MovementIntent.Offensive);
            _player.GetParty(0).Course.Tick();

            var b = _game.BattleService.GetBattle(party.Get<BattleGroupComponent>().BattleID);
            b.Task.Tick();
        }
    }
}