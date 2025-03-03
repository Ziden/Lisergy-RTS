using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Entities;
using Game.Events.ServerEvents;
using Game.Systems.Battler;
using Game.Systems.Course;
using Game.Systems.FogOfWar;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.World;
using NUnit.Framework;
using ServerTests;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Unit.Stubs;

namespace UnitTests
{
    public class TestMovement
    {
        private TestGame _game;
        private List<Location> _path;
        private TestServerPlayer _player;
        private IEntity _party;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _path = new List<Location>();
            _party = _player.GetParty(0);
        }

        private void SendMoveRequest()
        {
            var ev = new MoveEntityCommand() { Path = _path, Entity = _party.EntityId };
            ev.Sender = _player;
            _game.HandleClientEvent(_player, ev);
        }

        [Test]
        public void TestSettingCourseTask()
        {
            var tile = _party.GetTile();
            var next = tile.GetNeighbor(Direction.SOUTH);
            _path.Add(new Location(next.X, next.Y));

            SendMoveRequest();

            Assert.AreEqual(1, _game.GameScheduler.PendingTasks);
            Assert.IsTrue(_party.Logic.Movement.GetCourseTask() != null);
        }

        [Test]
        public void TestCourseMovingParty()
        {
            var date = DateTime.MinValue;
            _game.GameScheduler.SetLogicalTime(date);

            var tile = _party.Logic.Map.GetTile();
            var next = tile.GetNeighbor(Direction.SOUTH);
            _path.Add(new Location(next.X, next.Y));

            SendMoveRequest();

            Assert.AreEqual(tile, _party.GetTile());

            _game.GameScheduler.Tick(date + _party.Course().Delay);

            Assert.AreEqual(next, _party.GetTile());
        }

        /// <summary>
        /// When moving outside of a player vision, the player should still receive an entity update
        /// so he is aware this entity moved out. He would be aware by noticing the position component
        /// </summary>
        [Test]
        public void TestMoveOutsidePlayerVision()
        {
            var tile = _party.GetTile();
            var visibleTiles = _party.GetTile().GetAOE(_party.Get<EntityVisionComponent>().LineOfSight);
            var next = tile.GetNeighbor(Direction.SOUTH);

            var player2 = _game.CreatePlayer(5, 7);
            var player2Party = player2.GetParty(0); // placed in 8 - 7

            Assert.That(player2Party.GetTile().Logic.Vision.GetPlayersViewing().Contains(_player.EntityId));

            _game.Network.DeltaCompression.ClearDeltas();
            _player.ReceivedPackets.Clear();

            // Moving player2 to 5 7 which is slightly outside p1 vision
            _path.Add(new Location(5, 7));
            _game.HandleClientEvent(player2, new MoveEntityCommand() { Path = _path, Entity = player2Party.EntityId });
            _game.GameScheduler.Tick(_game.GameScheduler.Now + player2Party.Course().Delay);

            // p1 should still have received p2 component update even tho the entity is outside his vision because it was inside vision
            var moveEvents = _player.ReceivedPacketsOfType<EntityUpdatePacket>().Where(p => p.EntityId == player2Party.EntityId && p.SyncedComponents.Any(c => c is MapPlacementComponent));

            // should have received movement events
            Assert.AreEqual(1, moveEvents.Count());
        }

        [Test]
        public void TestMoveEvents()
        {
            var tile = _party.GetTile();
            var next = tile.GetNeighbor(Direction.SOUTH);
            _path.Add(new Location(next.X, next.Y));

            _game.Network.DeltaCompression.ClearDeltas();
            _player.ReceivedPackets.Clear();

            SendMoveRequest();
            _game.GameScheduler.Tick(_game.GameScheduler.Now + _party.Course().Delay);

            var moveEvents = _player.ReceivedPacketsOfType<EntityUpdatePacket>().Where(p => p.EntityId == _party.EntityId && p.SyncedComponents.Any(c => c is MapPlacementComponent));
            var tileDiscovery = _player.ReceivedEntityUpdates(EntityType.Tile);

            // should have received movement events
            Assert.AreEqual(1, moveEvents.Count());
        }

        [Test]
        public void TestCourseTask()
        {
            var tile = _party.GetTile();
            var next = tile.GetNeighbor(Direction.SOUTH);
            var party = _player.GetParty(0);

            _player.SendMoveRequest(party, next, CourseIntent.OffensiveTarget);
            var course = party.Course();

            course.Tick();
            course.Tick();

            Assert.AreEqual(party.GetTile(), next);
        }

        [Test]
        public void TestChangingCourse()
        {
            var tile = _party.GetTile();
            var next = tile.GetNeighbor(Direction.SOUTH);
            _path.Add(new Location(next.X, next.Y));

            SendMoveRequest();
            var course1 = _game.GameScheduler.Queue.First();

            _path.Add(new Location(next.X + 1, next.Y));
            SendMoveRequest();
            var course2 = _game.GameScheduler.Queue.First();

            Assert.AreNotEqual(course1, course2);
            Assert.IsFalse(_game.GameScheduler.Queue.Contains(course1));
            Assert.IsTrue(_game.GameScheduler.Queue.Contains(course2));
            Assert.IsFalse(course2.HasFinished);
        }

        [Test]
        public void TestCannotMoveBattlingUnit()
        {
            var tile = _party.GetTile();
            var next = tile.GetNeighbor(Direction.SOUTH);
            _path.Add(new Location(next.X, next.Y));
            var component = _party.Get<BattleGroupComponent>();
            component.BattleID = GameId.Generate();
            _party.Save(component);

            _path.Add(new Location(next.X + 1, next.Y));
            SendMoveRequest();

            Assert.AreEqual(GameId.ZERO, _party.Get<MovementComponent>().CourseId);
        }

        [Test]
        public void TestMultipleMoves()
        {
            var date = DateTime.MinValue;
            _game.GameScheduler.SetLogicalTime(date);

            var tile = _party.GetTile();
            var next1 = tile.GetNeighbor(Direction.SOUTH);
            var next2 = next1.GetNeighbor(Direction.SOUTH);
            var next3 = next2.GetNeighbor(Direction.SOUTH);
            _path.Add(new Location(next1.X, next1.Y));
            _path.Add(new Location(next2.X, next2.Y));
            _path.Add(new Location(next3.X, next3.Y));

            _game.Network.DeltaCompression.ClearDeltas();
            _player.ReceivedPackets.Clear();

            SendMoveRequest();

            Assert.AreEqual(tile, _party.GetTile());

            date += _party.Course().Delay;
            _game.GameScheduler.Tick(date);
            Assert.AreEqual(next1, _party.GetTile());

            date += _party.Course().Delay;
            _game.GameScheduler.Tick(date);
            Assert.AreEqual(next2, _party.GetTile());

            date += _party.Course().Delay;
            _game.GameScheduler.Tick(date);
            Assert.AreEqual(next3, _party.GetTile());

            var moveEvents = _player.ReceivedPacketsOfType<EntityUpdatePacket>().Where(p => p.EntityId == _party.EntityId && p.SyncedComponents.Any(c => c is MapPlacementComponent));
            Assert.AreEqual(3, moveEvents.Count());
        }
    }
}