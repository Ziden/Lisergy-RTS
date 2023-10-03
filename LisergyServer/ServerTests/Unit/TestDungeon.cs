using Game;
using Game.Battle;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Network.ClientPackets;
using Game.Network.ServerPackets;
using Game.Pathfinder;
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
        private List<MapPosition> _path;
        private TestServerPlayer _player;
        private PartyEntity _party;
        private DungeonEntity _dungeon;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _path = new List<MapPosition>();
            _party = _player.GetParty(0);
            _dungeon = new DungeonEntity();
            _dungeon.BuildFromSpec(GameLogic.Specs.Dungeons[0]);
            _dungeon.Tile = _game.World.GetTile(8, 8);
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
            var playerCastleTile = _player.Buildings.First().Tile;
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
            Assert.AreEqual(0, _player.BattleHeaders.Count());
        }

        [Test]
        public void TestAgressiveMoveOnDungeon()
        {
            var party = _player.GetParty(0);
            var partyTile = party.Tile;
            var dungeonTile = partyTile.GetNeighbor(Direction.EAST);

            _dungeon.Tile = dungeonTile;
            party.Components.Get<BattleGroupComponent>().Units.First().Atk = 255;

            _player.SendMoveRequest(party, dungeonTile, MovementIntent.Offensive);
            var course = party.Course;

            Assert.AreEqual(0, _player.BattleHeaders.Count());

            course.Tick();
            course.Tick();

            var battle = _game.BattleService.GetBattle(party.Components.Get<BattleGroupComponent>().BattleID);
            battle.Task.Tick();

            Assert.AreEqual(dungeonTile, party.Tile);
            Assert.AreEqual(1, _player.BattleHeaders.Count);
        }


        [Test]
        public void TestDungeonComponentsRegisterBuilding()
        {
            var playerCastleTile = _player.Buildings.First().Tile;
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);

            _dungeon.Tile = dungeonTile;

            var tileBuilding = dungeonTile.Components.Get<TileHabitants>().Building;
            Assert.That(tileBuilding.EntityId == _dungeon.Id);
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
            var playerCastleTile = _player.Buildings.First().Tile;
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
            var party = _player.GetParty(0);

            var hp1 = party.Components.Get<BattleGroupComponent>().Units.First().HP;

            party.Components.Get<BattleGroupComponent>().Units.First().HP = 1000; // make sure it wins !
            _dungeon.Tile = dungeonTile;

            var hp2 = party.Components.Get<BattleGroupComponent>().Units.First().HP;

            _player.SendMoveRequest(party, dungeonTile, MovementIntent.Offensive);
            _player.GetParty(0).Course.Tick();

            var battle = _game.BattleService.GetBattle(party.Components.Get<BattleGroupComponent>().BattleID);
            battle.Task.Tick();

            var result = _player.BattleHeaders.Values.First();

            //  Dungeon completed and removed from map
            Assert.IsTrue(!_dungeon.IsInMap);
            Assert.IsTrue(_dungeon.Get<BattleGroupComponent>().IsDestroyed);
            Assert.AreEqual(_dungeon.Tile, null);
            Assert.AreEqual(dungeonTile.Components.Get<TileHabitants>().Building, null);
            // Received another move event to remove the dungeon
            Assert.AreEqual(_player.ReceivedEventsOfType<EntityDestroyPacket>().Count, 1);
        }

        [Test]
        public void TestRevealingDungeonReceivesPacket()
        {
            _dungeon = new DungeonEntity();
            _dungeon.Get<BattleGroupComponent>().Units.Add(new Unit(1));
            _dungeon.Tile = _game.World.GetTile(_party.Tile.X + _party.GetLineOfSight() + 1, _party.Tile.Y);
            DeltaTracker.Clear();

            var seenClose = _dungeon.Tile.GetNeighbor(Direction.WEST);

            Assert.That(seenClose.EntitiesViewing.Contains(_party));
            Assert.That(!_dungeon.Tile.EntitiesViewing.Contains(_party));

            _player.ReceivedEvents.Clear();
            _party.Tile = _party.Tile.GetNeighbor(Direction.EAST);
            DeltaTracker.SendDeltaPackets(_player);

            Assert.That(_player.ReceivedEventsOfType<EntityUpdatePacket>().Any(p => p.Entity.GetType() == typeof(DungeonEntity)));
            Assert.That(_dungeon.Tile.EntitiesViewing.Contains(_party));
        }

        [Test]
        public void TestDungeonBattleTeamHasUnits()
        {
            _dungeon = new DungeonEntity();
            _dungeon.Get<BattleGroupComponent>().Units.Add(new Unit(1));

            var battleTeam = new BattleTeam(_dungeon, _dungeon.Get<BattleGroupComponent>().Units.ToArray());
            Assert.AreNotEqual(battleTeam.Units.Length, 0);
        }

        [Test]
        public void TestReceivingDungeonWhenLogin()
        {
            var joinEvent = new JoinWorldPacket();
            var clientPlayer = new TestServerPlayer(_game);
            _player.ReceivedEvents.Clear();

            var dg = new DungeonEntity();
            dg.Tile = _game.World.GetTile(4, 2);

            _game.HandleClientEvent(clientPlayer, joinEvent);

            var entityVisibleEvents = clientPlayer.ReceivedEventsOfType<EntityUpdatePacket>();

            Assert.That(entityVisibleEvents.Any(p => p.Entity.GetType() == typeof(DungeonEntity)));
            Assert.AreEqual(3, entityVisibleEvents.Count, "Initial Party & Building should be visible");
        }

        [Test]
        public void TestPartyDefeatHealsDungeon()
        {
            var playerCastleTile = _player.Buildings.First().Tile;
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