using Game;
using Game.ECS;
using Game.Entity.Entities;
using Game.Events;
using Game.Events.ServerEvents;
using Game.Movement;
using Game.Scheduler;
using Game.World;
using Game.World.Components;
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
            _dungeon = new DungeonEntity();
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
            Assert.IsTrue(party.BattleLogic.BattleID.IsZero());
            Assert.AreEqual(0, _player.Battles.Count());
        }

        [Test]
        public void TestAgressiveMoveOnDungeon()
        {
            var playerCastleTile = _player.Buildings.First().Tile;
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
            var party = _player.GetParty(0);
            party.BattleLogic.GetUnits().First().Atk = 255;

            dungeonTile.Components.Get<TileHabitants>().Building = _dungeon;

            _player.SendMoveRequest(_player.GetParty(0), dungeonTile, MovementIntent.Offensive);
            var course = party.Course;

            Assert.AreEqual(0, _player.Battles.Count());

            course.Execute();

            var battle = _game.BattleService.GetBattle(party.BattleLogic.BattleID);
            battle.Task.Execute();

            Assert.AreEqual(dungeonTile, _player.GetParty(0).Tile);


            Assert.AreEqual(1, _player.Battles.Count);
        }


        [Test]
        public void TestDungeonComponentsRegisterBuilding()
        {
            var playerCastleTile = _player.Buildings.First().Tile;
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
          
            _dungeon.Tile = dungeonTile;

            Assert.That(dungeonTile.Components.Get<TileHabitants>().Building.Id == _dungeon.Id);  
        }

        [Test]
        public void TestDungeonRemovedWhenComplete()
        {
            var playerCastleTile = _player.Buildings.First().Tile;
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
            var party = _player.GetParty(0);
            party.BattleLogic.GetUnits().First().Atk = 255; // make sure it wins !
            _dungeon.Tile = dungeonTile;

            _player.SendMoveRequest(_player.GetParty(0), dungeonTile, MovementIntent.Offensive);
            _player.GetParty(0).Course.Execute();

            var battle = _game.BattleService.GetBattle(party.BattleLogic.BattleID);
            battle.Task.Execute();

            //  Dungeon completed and removed from map
            Assert.IsTrue(!_dungeon.IsInMap);
            Assert.IsTrue(_dungeon.BattleLogic.IsDestroyed);
            Assert.AreEqual(_dungeon.Tile, null);
            Assert.AreEqual(dungeonTile.Components.Get<TileHabitants>().Building, null);
            // Received another move event to remove the dungeon
            Assert.AreEqual(_player.ReceivedEventsOfType<EntityDestroyPacket>().Count, 1);
        }

        [Test]
        public void TestRevealingDungeonReceivesPacket()
        {
            _dungeon = new DungeonEntity(new Unit(1));
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
        public void TestReceivingDungeonWhenLogin()
        {
            var joinEvent = new JoinWorldPacket();
            var clientPlayer = new TestServerPlayer();
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
            party.BattleLogic.GetUnits().First().HP = 1; // make sure it looses !
            party.BattleLogic.GetUnits().First().Atk = 0; // make sure it looses !
            dungeonTile.Components.Get<TileHabitants>().Building = _dungeon;

            _player.SendMoveRequest(_player.GetParty(0), dungeonTile, MovementIntent.Offensive);
            _player.GetParty(0).Course.Execute();

            var b = _game.BattleService.GetBattle(party.BattleLogic.BattleID);
            b.Task.Execute();
            
            /*
            //  Dungeon completed and removed from map
            Assert.IsFalse(_dungeon.IsComplete());
            foreach (var battle in _dungeon.Battles)
                foreach (var unit in battle)
                    Assert.AreEqual(unit.Stats.HP, unit.Stats.MaxHP);

            // Party is recalled to castle
            Assert.AreEqual(party.Tile, playerCastleTile);
            */
        }
    }
}