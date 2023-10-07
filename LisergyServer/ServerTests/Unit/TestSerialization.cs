using Game;
using Game.Battle;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using Game.Network.ServerPackets;
using Game.Systems.Battler;
using GameData.Specs;
using NUnit.Framework;
using ServerTests;
using System;
using System.Linq;

namespace Tests
{
    public class TestSerialization
    {

        private TestGame _game;

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
            var bytes = Serialization.FromPacket<AuthPacket>(authEvent);
            var event2 = Serialization.ToPacket<AuthPacket>(bytes);

            Assert.AreEqual(authEvent.Login, event2.Login);
            Assert.AreEqual(authEvent.Password, event2.Password);
        }

        [Test]
        public void TestTileSerialization()
        {

            var player = _game.GetTestPlayer();
            var tile = _game.World.GetTile(1, 1);

            Serialization.LoadSerializers(typeof(TilePacket));

            var serialized = Serialization.FromPacket<TilePacket>(tile.GetUpdatePacket(null) as TilePacket);
            var unserialized = Serialization.ToPacket<TilePacket>(serialized);

            Assert.AreEqual(tile.SpecId, unserialized.Data.TileId);
            Assert.AreEqual(tile.X, unserialized.Data.X);
            Assert.AreEqual(tile.Y, unserialized.Data.Y);
        }

        [Test]
        public void TestUnitViewEventSerialization()
        {
            var game = new TestGame();

            var player = game.GetTestPlayer();
            var unit = player.Data.Units.First();
            var building = player.Data.Buildings.First();
            var tile = player.Data.Parties[0].Tile;

            var visibleEvent = game.SentPackets.Where(e => e is EntityUpdatePacket).FirstOrDefault() as EntityUpdatePacket;

            var serialized = Serialization.FromPacket<EntityUpdatePacket>(visibleEvent);
            var unserialized = Serialization.ToPacket<EntityUpdatePacket>(serialized);

            Assert.AreEqual(visibleEvent.EntityId, unserialized.EntityId);
        }

        [Test]
        public void TestEntityUpdatePacket()
        {
            var game = new TestGame();

            var player = game.GetTestPlayer();
            var party = player.Data.Parties[0];

            var entityUpdate = new EntityUpdatePacket(party);

            var serialized = Serialization.FromPacket<EntityUpdatePacket>(entityUpdate);
            var unserialized = Serialization.ToPacket<EntityUpdatePacket>(serialized);

            Assert.AreEqual(unserialized.EntityId, party.EntityId);
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
            var bytes = Serialization.FromPacketRaw(authEvent);
            var event2 = (AuthPacket)Serialization.ToPacketRaw(bytes);

            Assert.AreEqual(authEvent.Login, event2.Login);
            Assert.AreEqual(authEvent.Password, event2.Password);
        }

        [Test]
        public void TestBattleLogSerialization()
        {
            Serialization.LoadSerializers();
            var enemyTeam = new BattleTeam(new Unit(_game.Specs.Units[0]), new Unit(_game.Specs.Units[0]));
            var myTeam = new BattleTeam(new Unit(_game.Specs.Units[2]), new Unit(_game.Specs.Units[0]));
            var battle = new TurnBattle(Guid.NewGuid(), myTeam, enemyTeam);
            var log = new BattleLogPacket(battle);
            var autoRun = new AutoRun(battle);
            var result = autoRun.RunAllRounds();
            log.SetTurns(result);

            var deserializedHeader = log.DeserializeStartingState();

            Assert.AreEqual(deserializedHeader.Attacker.Units.First().UnitID, myTeam.Units.First().UnitID);
            Assert.AreEqual(deserializedHeader.Defender.Units.First().UnitID, enemyTeam.Units.First().UnitID);
        }
    }
}