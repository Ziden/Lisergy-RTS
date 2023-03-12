using Game.Events;
using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace Tests
{
    public class TestJoinWorld
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame(createPlayer:false);
            Serialization.LoadSerializers();
        }

        [Test]
        public void TestJoinCreatingPlayerData()
        {
            var playersBefore = Game.World.Players.PlayerCount;
            var joinEvent = new JoinWorldPacket();
            var clientPlayer = new TestServerPlayer();
            Game.HandleClientEvent(clientPlayer, joinEvent);

            var createdPlayer = Game.World.Players.GetPlayer(clientPlayer.UserID);

            Assert.AreEqual(1, Game.World.Players.PlayerCount);
            Assert.AreEqual(clientPlayer, createdPlayer);
            Assert.AreEqual(1, createdPlayer.Buildings.Count);  // initial building
            Assert.IsTrue(createdPlayer.GetParty(0) != null);    // initial party
        }

        [Test]
        public void TestJoinInitialDataEventsSent()
        {
            var playersBefore = Game.World.Players.PlayerCount;
            var joinEvent = new JoinWorldPacket();
            var clientPlayer = new TestServerPlayer();
            Game.HandleClientEvent(clientPlayer, joinEvent);

            var entityVisibleEvents = clientPlayer.ReceivedEventsOfType<EntityUpdatePacket>();

            Assert.AreEqual(2, entityVisibleEvents.Count, "Initial Party & Building should be visible");
        }

        [Test]
        public void TestNewPlayerReceivingEvents()
        {
            var playersBefore = Game.World.Players.PlayerCount;
            var joinEvent = new JoinWorldPacket();
            var player = new TestServerPlayer();
            Game.HandleClientEvent(player, joinEvent);

            var entityVisibleEvents = player.ReceivedEventsOfType<EntityUpdatePacket>();
            var tileVisibleEvent = player.ReceivedEventsOfType<TileVisiblePacket>();

            Assert.IsTrue(tileVisibleEvent.Count > 2);
            Assert.AreEqual(2, entityVisibleEvents.Count);
            Assert.IsTrue(entityVisibleEvents.Where(e => e.Entity == player.GetParty(0)).Any());
            Assert.IsTrue(entityVisibleEvents.Where(e => e.Entity == player.Buildings.First()).Any());
        }

        [Test]
        public void TestJoinExistingPlayer()
        {
            var playersBefore = Game.World.Players.PlayerCount;
            var joinEvent = new JoinWorldPacket();
            var player = new TestServerPlayer();
            Game.HandleClientEvent(player, joinEvent);

            var firstEntityVisibleEvents = player.ReceivedEventsOfType<EntityUpdatePacket>();
            var firstTileVisibleEvents = player.ReceivedEventsOfType<TileVisiblePacket>();

            player.ReceivedEvents.Clear();

            Game.HandleClientEvent(player, joinEvent);

            var secondEntityVisibleEvents = player.ReceivedEventsOfType<EntityUpdatePacket>();
            var secondTileVisibleEvents = player.ReceivedEventsOfType<TileVisiblePacket>();

            Assert.AreEqual(firstEntityVisibleEvents.Count, secondEntityVisibleEvents.Count);
            Assert.AreEqual(firstTileVisibleEvents.Count, secondTileVisibleEvents.Count);
        }

        [Test]
        public void TestInitialUnitStats()
        {
            var playersBefore = Game.World.Players.PlayerCount;
            var joinEvent = new JoinWorldPacket();
            var player = new TestServerPlayer();
            Game.HandleClientEvent(player, joinEvent);
            var party = player.GetParty(0);
            var unit = party.GetUnits()[0];
          
            Assert.That(unit.Stats.HP == unit.Stats.MaxHP);
        }
    }
}