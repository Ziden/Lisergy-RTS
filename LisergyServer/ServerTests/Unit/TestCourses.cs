using Game;
using Game.Entity;
using Game.Events;
using Game.Events.ServerEvents;
using Game.Scheduler;
using Game.World;
using NUnit.Framework;
using ServerTests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class TestMovement
    {
        private TestGame _game;
        private List<Position> _path;
        private TestServerPlayer _player;
        private Party _party;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _path = new List<Position>();
            _party = _player.GetParty(0);
            GameScheduler.Clear();
        }

        [TearDown]
        public void Tear()
        {
            _game.ClearEventListeners();
        }

        private void SendMoveRequest()
        {
            var ev = new MoveRequestPacket() { Path = _path, PartyIndex = _party.PartyIndex };
            ev.Sender = _player;
            _game.HandleClientEvent(_player, ev);
        }

        [Test]
        public void TestSettingCourseTask()
        {
            var tile = _party.Tile;
            var next = tile.GetNeighbor(Direction.SOUTH);
            _path.Add(new Position(next.X, next.Y));

            SendMoveRequest();

            Assert.AreEqual(1, GameScheduler.PendingTasks);
            Assert.IsTrue(_party.Course != null);
        }

        [Test]
        public void TestCourseMovingParty()
        {
            var date = DateTime.MinValue;
            GameScheduler.SetLogicalTime(date);

            var tile = _party.Tile;
            var next = tile.GetNeighbor(Direction.SOUTH);
            _path.Add(new Position(next.X, next.Y));

            SendMoveRequest();

            Assert.AreEqual(tile, _party.Tile);

            GameScheduler.Tick(date + _party.Course.Delay);

            Assert.AreEqual(next, _party.Tile);
        }

        [Test]
        public void TestMoveEvents()
        {
            var tile = _party.Tile;
            var next = tile.GetNeighbor(Direction.SOUTH);
            _path.Add(new Position(next.X, next.Y));

            var moveEventsb = _player.ReceivedEventsOfType<EntityMovePacket>();

            SendMoveRequest();
            GameScheduler.Tick(GameScheduler.Now + _party.Course.Delay);

            var moveEvents = _player.ReceivedEventsOfType<EntityMovePacket>();
            var tileDiscovery = _player.ReceivedEventsOfType<TileUpdatePacket>();
            // should have received movement events
            Assert.AreEqual(1, moveEvents.Count);
            // should have explored some tiles
            Assert.GreaterOrEqual(tileDiscovery.Count, 1);
        }

        [Test]
        public void TestChangingCourse()
        {
            var tile = _party.Tile;
            var next = tile.GetNeighbor(Direction.SOUTH);
            _path.Add(new Position(next.X, next.Y));

            SendMoveRequest();
            var course1 = GameScheduler.Queue.First();

            _path.Add(new Position(next.X+1, next.Y));
            SendMoveRequest();
            var course2 = GameScheduler.Queue.First();

            Assert.AreNotEqual(course1, course2);
            Assert.IsFalse(GameScheduler.Queue.Contains(course1));
            Assert.IsTrue(GameScheduler.Queue.Contains(course2));
            Assert.IsTrue(course1.HasFinished);
            Assert.IsFalse(course2.HasFinished);
        }

        [Test]
        public void TestCannotMoveBattlingUnit()
        {
            var tile = _party.Tile;
            var next = tile.GetNeighbor(Direction.SOUTH);
            _path.Add(new Position(next.X, next.Y));
            _party.BattleID = Guid.NewGuid();

            _path.Add(new Position(next.X + 1, next.Y));
            SendMoveRequest();

            Assert.IsNull(_party.Course);
        }

        [Test]
        public void TestMultipleMoves()
        {
            var date = DateTime.MinValue;
            GameScheduler.SetLogicalTime(date);

            var tile = _party.Tile;
            var next1 = tile.GetNeighbor(Direction.SOUTH);
            var next2 = next1.GetNeighbor(Direction.SOUTH);
            var next3 = next2.GetNeighbor(Direction.SOUTH);
            _path.Add(new Position(next1.X, next1.Y));
            _path.Add(new Position(next2.X, next2.Y));
            _path.Add(new Position(next3.X, next3.Y));

            SendMoveRequest();

            Assert.AreEqual(tile, _party.Tile);

            date += _party.Course.Delay;
            GameScheduler.Tick(date);
            Assert.AreEqual(next1, _party.Tile);

            date += _party.Course.Delay;
            GameScheduler.Tick(date);
            Assert.AreEqual(next2, _party.Tile);

            date += _party.Course.Delay;
            GameScheduler.Tick(date);
            Assert.AreEqual(next3, _party.Tile);

            var moveEvents = _player.ReceivedEventsOfType<EntityMovePacket>();
            Assert.AreEqual(3, moveEvents.Count);
        }
    }
}