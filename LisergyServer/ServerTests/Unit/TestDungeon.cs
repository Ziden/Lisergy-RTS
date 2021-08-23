using BattleServer;
using Game;
using Game.Entity;
using Game.Events;
using Game.Movement;
using Game.Scheduler;
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
        private Party _party;
        private Dungeon _dungeon;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _path = new List<Position>();
            _party = _player.GetParty(0);
            _dungeon = new Dungeon(new Unit(1));
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
            dungeonTile.StaticEntity = _dungeon;
            
            // send intent to move player to the party
            _player.SendMoveRequest(party, dungeonTile, MovementIntent.Defensive);
            var course = _player.GetParty(0).Course;

            // Complete the move intent
            GameScheduler.ForceComplete(course);

            Assert.AreEqual(dungeonTile, party.Tile);
            Assert.AreEqual(party.BattleID, null);
            Assert.AreEqual(0, _player.Battles.Count());
        }

        [Test]
        public void TestAgressiveMoveOnDungeon()
        {
            var playerCastleTile = _player.Buildings.First().Tile;
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
            var party = _player.GetParty(0);
            party.GetUnits()[0].Stats.Atk = 9999;

            dungeonTile.StaticEntity = _dungeon;

            _player.SendMoveRequest(_player.GetParty(0), dungeonTile, MovementIntent.Offensive);
            var course = party.Course;

            Assert.AreEqual(0, _player.Battles.Count());

            course.Execute();

            var battle = _game.GetListener<BattlePacketListener>().GetBattle(party.BattleID);
            battle.Task.Execute();

            Assert.AreEqual(dungeonTile, _player.GetParty(0).Tile);


            Assert.AreEqual(1, _player.Battles.Count);
        }

        [Test]
        public void TestDungeonRemovedWhenComplete()
        {
            var playerCastleTile = _player.Buildings.First().Tile;
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
            var party = _player.GetParty(0);
            party.GetUnits()[0].Stats.Atk = 30000; // make sure it wins !
            _dungeon.Tile = dungeonTile;

            _player.SendMoveRequest(_player.GetParty(0), dungeonTile, MovementIntent.Offensive);
            _player.GetParty(0).Course.Execute();

            var battle = _game.GetListener<BattlePacketListener>().GetBattle(party.BattleID);
            battle.Task.Execute();

            //  Dungeon completed and removed from map
            Assert.IsTrue(_dungeon.IsComplete());
            Assert.AreEqual(_dungeon.Tile, null);
            Assert.AreEqual(dungeonTile.StaticEntity, null);
            // Received another move event to remove the dungeon
            Assert.AreEqual(_player.ReceivedEventsOfType<EntityDestroyPacket>().Count, 1);
        }

        [Test]
        public void TestPartyDefeatHealsDungeon()
        {
            var playerCastleTile = _player.Buildings.First().Tile;
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
            var party = _player.GetParty(0);
            party.GetUnits()[0].Stats.HP = 1; // make sure it looses !
            party.GetUnits()[0].Stats.Atk = 0; // make sure it looses !
            dungeonTile.StaticEntity = _dungeon;

            _player.SendMoveRequest(_player.GetParty(0), dungeonTile, MovementIntent.Offensive);
            _player.GetParty(0).Course.Execute();

            var b = _game.GetListener<BattlePacketListener>().GetBattle(party.BattleID);
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