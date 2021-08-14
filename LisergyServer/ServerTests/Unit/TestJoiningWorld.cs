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
            var joinEvent = new JoinWorldEvent();
            var clientPlayer = new TestServerPlayer();
            Game.HandleClientEvent(clientPlayer, joinEvent);

            var createdPlayer = Game.World.Players.GetPlayer(clientPlayer.UserID);

            Assert.AreEqual(1, Game.World.Players.PlayerCount);
            Assert.AreEqual(clientPlayer, createdPlayer);
            Assert.AreEqual(1, createdPlayer.Buildings.Count);  // initial building
            Assert.IsTrue(createdPlayer.Parties[0] != null);    // initial party
        }

        [Test]
        public void TestNewPlayerReceivingEvents()
        {
            var playersBefore = Game.World.Players.PlayerCount;
            var joinEvent = new JoinWorldEvent();
            var player = new TestServerPlayer();
            Game.HandleClientEvent(player, joinEvent);

            var entityVisibleEvents = player.ReceivedEventsOfType<EntityVisibleEvent>();
            var tileVisibleEvent = player.ReceivedEventsOfType<TileVisibleEvent>();

            Assert.IsTrue(tileVisibleEvent.Count > 2);
            Assert.AreEqual(2, entityVisibleEvents.Count);
            Assert.IsTrue(entityVisibleEvents.Where(e => e.Entity == player.Parties.First()).Any());
            Assert.IsTrue(entityVisibleEvents.Where(e => e.Entity == player.Buildings.First()).Any());
        }

        [Test]
        public void TestJoinExistingPlayer()
        {
            var playersBefore = Game.World.Players.PlayerCount;
            var joinEvent = new JoinWorldEvent();
            var player = new TestServerPlayer();
            Game.HandleClientEvent(player, joinEvent);

            var firstEntityVisibleEvents = player.ReceivedEventsOfType<EntityVisibleEvent>();
            var firstTileVisibleEvents = player.ReceivedEventsOfType<TileVisibleEvent>();

            player.ReceivedEvents.Clear();

            Game.HandleClientEvent(player, joinEvent);

            var secondEntityVisibleEvents = player.ReceivedEventsOfType<EntityVisibleEvent>();
            var secondTileVisibleEvents = player.ReceivedEventsOfType<TileVisibleEvent>();

            Assert.AreEqual(firstEntityVisibleEvents.Count, secondEntityVisibleEvents.Count);
            Assert.AreEqual(firstTileVisibleEvents.Count, secondTileVisibleEvents.Count);
        }

        [Test]
        public void TestInitialUnitStats()
        {
            var playersBefore = Game.World.Players.PlayerCount;
            var joinEvent = new JoinWorldEvent();
            var player = new TestServerPlayer();
            Game.HandleClientEvent(player, joinEvent);
            var party = player.Parties[0];
            var unit = party.GetUnits()[0];
          

            Assert.That(unit.Stats.HP == unit.Stats.MaxHP);
        }
    }
}