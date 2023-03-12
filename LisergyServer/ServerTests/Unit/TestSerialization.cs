using Game;
using Game.Battles;
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
        public class TestTileEvent : BaseEvent
        {
            public TestTileEvent(Tile t)
            {
                this.Tile = t;
            }
            public Tile Tile;
        }

        [Serializable]
        public class RefEvent : BaseEvent
        {
            public BattleTeam T1;
            public BattleTeam T2;   
        }

        [Test]
        public void TestSimpleSerialization()
        {
            Serialization.LoadSerializers();
            var authEvent = new AuthPacket()
            {
                Login = "wololo",
                Password = "walala"
            };
            var bytes = Serialization.FromEvent<AuthPacket>(authEvent);
            var event2 = Serialization.ToEvent<AuthPacket>(bytes);

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

            var visibleEvent = game.ReceivedEvents.Where(e => e is EntityUpdatePacket).FirstOrDefault() as EntityUpdatePacket;

            var serialized = Serialization.FromEvent<EntityUpdatePacket>(visibleEvent);
            var unserialized = Serialization.ToEvent<EntityUpdatePacket>(serialized);

            Assert.AreEqual(visibleEvent.Entity.Id, unserialized.Entity.Id);
        }

        [Test]
        public void TestRawSerialization()
        {
            Serialization.LoadSerializers();
            var authEvent = new AuthPacket()
            {
                Login = "wololo",
                Password = "walala"
            };
            var bytes = Serialization.FromEventRaw(authEvent);
            var event2 = (AuthPacket) Serialization.ToEventRaw(bytes);

            Assert.AreEqual(authEvent.Login, event2.Login);
            Assert.AreEqual(authEvent.Password, event2.Password);
        }
    }
}