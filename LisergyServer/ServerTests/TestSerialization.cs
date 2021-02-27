using Game;
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
            Assert.AreEqual(tile.BuildingID, unserialized.Tile.BuildingID);
            Assert.AreEqual(tile.ResourceID, unserialized.Tile.ResourceID);
            Assert.AreEqual(tile.UserID, unserialized.Tile.UserID);
            Assert.AreEqual(tile.X, unserialized.Tile.X);
            Assert.AreEqual(tile.Y, unserialized.Tile.Y);
        }

        [Test]
        public void TestUnitViewEventSerialization()
        {
            var game = new TestGame();
            Serialization.LoadSerializers(typeof(PartyVisibleEvent));

            var player = game.GetTestPlayer();
            var unit = player.Units.First();
            var building = player.Buildings.First();
            var tile = unit.Party.Tile;

            var visibleEvent = game.ReceivedEvents.Where(e => e is PartyVisibleEvent).FirstOrDefault() as PartyVisibleEvent;

            var serialized = Serialization.FromEvent<PartyVisibleEvent>(visibleEvent);
            var unserialized = Serialization.ToEvent<PartyVisibleEvent>(serialized);

            Assert.AreEqual(visibleEvent.Party.Id, unserialized.Party.Id);
        }

        [Test]
        public void TestTileWIthData()
        {
            var game = new TestGame();
            var player = game.GetTestPlayer();
            var tile = game.RandomNotBuiltTile();
            Serialization.LoadSerializers(typeof(TestTileEvent));
            var buildingSpec = TestGame.RandomBuildingSpec();

            tile.Building = new Building(buildingSpec.Id, player);
           
            var serialized = Serialization.FromEvent<TestTileEvent>(new TestTileEvent(tile));
            var unserialized = Serialization.ToEvent<TestTileEvent>(serialized);

            Assert.AreEqual(tile.BuildingID, unserialized.Tile.BuildingID);

            Assert.AreEqual(tile.ResourceID, unserialized.Tile.ResourceID);
            Assert.AreEqual(tile.UserID, unserialized.Tile.UserID);
            Assert.AreEqual(tile.X, unserialized.Tile.X);
            Assert.AreEqual(tile.Y, unserialized.Tile.Y);
        }
    }
}