using Game.Events;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using Game.Network.ServerPackets;
using Game.Pathfinder;
using Game.Scheduler;
using Game.Systems.Battler;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Systems.Tile;
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
        private PartyEntity _party;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _path = new List<Position>();
            _party = _player.GetParty(0);
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

            Assert.AreEqual(1, _game.GameScheduler.PendingTasks);
            Assert.IsTrue(_party.Course != null);
        }

        [Test]
        public void TestCourseMovingParty()
        {
            var date = DateTime.MinValue;
            _game.GameScheduler.SetLogicalTime(date);

            var tile = _party.Tile;
            var next = tile.GetNeighbor(Direction.SOUTH);
            _path.Add(new Position(next.X, next.Y));

            SendMoveRequest();

            Assert.AreEqual(tile, _party.Tile);

            _game.GameScheduler.Tick(date + _party.Course.Delay);

            Assert.AreEqual(next, _party.Tile);
        }

        [Test]
        public void TestMoveEvents()
        {
            var tile = _party.Tile;
            var next = tile.GetNeighbor(Direction.SOUTH);
            _path.Add(new Position(next.X, next.Y));

            SendMoveRequest();
            _game.GameScheduler.Tick(_game.GameScheduler.Now + _party.Course.Delay);

            var moveEvents = _player.ReceivedPacketsOfType<EntityMovePacket>();
            var tileDiscovery = _player.ReceivedPacketsOfType<TilePacket>();
            // should have received movement events
            Assert.AreEqual(1, moveEvents.Count);
            // should have explored some tiles
            Assert.GreaterOrEqual(tileDiscovery.Count, 1);
        }

        [Test]
        public void TestCourseTask()
        {
            var tile = _party.Tile;
            var next = tile.GetNeighbor(Direction.SOUTH);
            var party = _player.GetParty(0);

            _player.SendMoveRequest(party, next, MovementIntent.Offensive);
            var course = party.Course;

            course.Tick();
            course.Tick();

            Assert.AreEqual(party.Tile, next);
        }

        [Test]
        public void TestChangingCourse()
        {
            var tile = _party.Tile;
            var next = tile.GetNeighbor(Direction.SOUTH);
            _path.Add(new Position(next.X, next.Y));

            SendMoveRequest();
            var course1 = _game.GameScheduler.Queue.First();

            _path.Add(new Position(next.X + 1, next.Y));
            SendMoveRequest();
            var course2 = _game.GameScheduler.Queue.First();

            Assert.AreNotEqual(course1, course2);
            Assert.IsFalse(_game.GameScheduler.Queue.Contains(course1));
            Assert.IsTrue(_game.GameScheduler.Queue.Contains(course2));
            Assert.IsTrue(course1.HasFinished);
            Assert.IsFalse(course2.HasFinished);
        }

        [Test]
        public void TestCannotMoveBattlingUnit()
        {
            var tile = _party.Tile;
            var next = tile.GetNeighbor(Direction.SOUTH);
            _path.Add(new Position(next.X, next.Y));
            _party.Get<BattleGroupComponent>().BattleID = Guid.NewGuid();

            _path.Add(new Position(next.X + 1, next.Y));
            SendMoveRequest();

            Assert.IsNull(_party.Course);
        }

        [Test]
        public void TestMultipleMoves()
        {
            var date = DateTime.MinValue;
            _game.GameScheduler.SetLogicalTime(date);

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
            _game.GameScheduler.Tick(date);
            Assert.AreEqual(next1, _party.Tile);

            date += _party.Course.Delay;
            _game.GameScheduler.Tick(date);
            Assert.AreEqual(next2, _party.Tile);

            date += _party.Course.Delay;
            _game.GameScheduler.Tick(date);
            Assert.AreEqual(next3, _party.Tile);

            var moveEvents = _player.ReceivedPacketsOfType<EntityMovePacket>();
            Assert.AreEqual(3, moveEvents.Count);
        }
    }
}