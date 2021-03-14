using Game;
using Game.Battle;
using Game.Events;
using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests;
using System;
using System.Linq;

namespace Tests
{
    public class TestSerialization
    {
        [Serializable]
        public class TestTileEvent : GameEvent
        {
            public TestTileEvent(Tile t)
            {
                this.Tile = t;
            }

            public override EventID GetID() => EventID.AUTH;
            public Tile Tile;
        }

        [Serializable]
        public class RefEvent : GameEvent
        {
            public override EventID GetID() => EventID.AUTH;

            public BattleTeam T1;
            public BattleTeam T2;
       
        }

        [Test]
        public void TestSimpleSerialization()
        {
            Serialization.LoadSerializers();
            var authEvent = new AuthEvent()
            {
                Login = "wololo",
                Password = "walala"
            };
            var bytes = Serialization.FromEvent<AuthEvent>(authEvent);
            var event2 = Serialization.ToEvent<AuthEvent>(bytes);

            Assert.AreEqual(authEvent.Login, event2.Login);
            Assert.AreEqual(authEvent.Password, event2.Password);
        }

        [Test]
        public void TestTileSerialization()
        {
            var game = new TestGame();
            var player = game.GetTestPlayer();
            var tile = game.World.GetTile(1, 1);
            tile.ResourceID = 123;
            Serialization.LoadSerializers(typeof(TestTileEvent));

            var serialized = Serialization.FromEvent<TestTileEvent>(new TestTileEvent(tile));
            var unserialized = Serialization.ToEvent<TestTileEvent>(serialized);

            Assert.AreEqual(tile.TileId, unserialized.Tile.TileId);
            Assert.AreEqual(tile.ResourceID, unserialized.Tile.ResourceID);
            Assert.AreEqual(tile.X, unserialized.Tile.X);
            Assert.AreEqual(tile.Y, unserialized.Tile.Y);
        }

        [Test]
        public void TestUnitViewEventSerialization()
        {
            var game = new TestGame();

            var player = game.GetTestPlayer();
            var unit = player.Units.First();
            var building = player.Buildings.First();
            var tile = unit.Party.Tile;

            var visibleEvent = game.ReceivedEvents.Where(e => e is EntityVisibleEvent).FirstOrDefault() as EntityVisibleEvent;

            var serialized = Serialization.FromEvent<EntityVisibleEvent>(visibleEvent);
            var unserialized = Serialization.ToEvent<EntityVisibleEvent>(serialized);

            Assert.AreEqual(visibleEvent.Entity.Id, unserialized.Entity.Id);
        }
    }
}