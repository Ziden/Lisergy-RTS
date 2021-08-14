using Game;
using Game.Battles;
using Game.Entity;
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
            _party = _player.Parties[0];
            _dungeon = new Dungeon(new Unit(0));
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
            var party = _player.Parties.First();

            // place the dungeon
            dungeonTile.StaticEntity = _dungeon;
            
            // send intent to move player to the party
            _player.SendMoveRequest(party, dungeonTile, MovementIntent.Defensive);
            var course = _player.Parties.First().Course;

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
            var party = _player.Parties.First();

            dungeonTile.StaticEntity = _dungeon;

            _player.SendMoveRequest(_player.Parties.First(), dungeonTile, MovementIntent.Offensive);
            var course = _player.Parties.First().Course;

            Assert.AreEqual(0, _player.Battles.Count());
            course.Execute();

            Assert.AreEqual(dungeonTile, _player.Parties.First().Tile);


            Assert.AreEqual(1, _player.Battles.Count);
        }
    }
}