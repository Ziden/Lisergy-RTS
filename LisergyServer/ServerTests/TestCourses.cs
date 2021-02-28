using Game;
using Game.Entity;
using Game.Events;
using Game.Scheduler;
using Game.World;
using NUnit.Framework;
using ServerTests;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class TestCourses
    {
        private TestGame _game;
        private List<Position> _path;
        private PlayerEntity _player;
        private Party _party;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _path = new List<Position>();
            _party = _player.Parties[0];
            GameScheduler.Clear();
        }

        [TearDown]
        public void Tear()
        {
            _game.ClearEventListeners();
        }

        private void SendMoveRequest()
        {
            var ev = new MoveRequestEvent() { Path = _path, PartyIndex = _party.PartyIndex };
            ev.Sender = _player;
            ev.FromNetwork = true;
            NetworkEvents.RequestPartyMove(ev);
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
    }
}