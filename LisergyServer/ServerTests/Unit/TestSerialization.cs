using Game;
using Game.Battles;
using Game.Events;
using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Tests
{
    public class TestSerialization
    {

        private TestGame _game;

        [Serializable]
        public class TestTileEvent : BaseEvent
        {
            public TestTileEvent(Tile t)
            {
                this.Tile = t;
            }
            public Tile Tile;
        }

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
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
         
            var player = _game.GetTestPlayer();
            var tile = _game.World.GetTile(1, 1);
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
        public void TestEntityUpdatePacket()
        {
            var game = new TestGame();

            var player = game.GetTestPlayer();
            var party = player.Parties[0];

            var entityUpdate = new EntityUpdatePacket(party);

            var serialized = Serialization.FromEvent<EntityUpdatePacket>(entityUpdate);
            var unserialized = Serialization.ToEvent<EntityUpdatePacket>(serialized);

            Assert.AreEqual(unserialized.Entity.Id, party.Id);
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


        [Test]
        public void TestSerializationSizes()
        {
            var testData = new Dictionary<Type, byte[]>();

            void Record<T>(T obj)
            {
                testData[typeof(T)] = Serialization.FromAnyType(obj);
            }   

            Record(new TileUpdatePacket(_game.World.GetTile(1, 1)));
            Record(new PartyStatusUpdatePacket(_game.GetTestPlayer().GetParty(0)));
            Record(new BattleResultPacket(Guid.NewGuid(), new TurnBattleResult() { Turns = new List<Game.Battles.Actions.TurnLog>(10)}));
            Record(new EntityDestroyPacket(_game.GetTestPlayer().GetParty(0)));
            Record(new EntityMovePacket(_game.GetTestPlayer().GetParty(0), _game.World.GetTile(1, 1)));
            Record(new MessagePopupPacket(PopupType.BAD_INPUT, "Yeah this is a message popup to test our serialization sizes"));
            Record(new BattleStartPacket(GameId.Generate(), _game.GetTestPlayer().GetParty(0), _game.GetTestPlayer().GetParty(0)));
            Record(new BattleTeam(new Unit(0), new Unit(0), new Unit(0), new Unit(0)));
            Record(new Unit(0));
            Record(GameId.Generate());

            var t = testData[typeof(BattleTeam)];
            var t2 = testData[typeof(Unit)];
            var t3 = testData[typeof(GameId)];

            foreach (var kp in testData)
            {
                Assert.LessOrEqual(kp.Value.Count(), 150, $"Packets should be lower then 100 bytes but {kp.Key} was not");
            }
        }
    }
}