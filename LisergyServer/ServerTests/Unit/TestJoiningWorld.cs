using Game.Engine;
using Game.Entities;
using Game.Events.ServerEvents;
using Game.Systems.Battler;
using Game.Systems.Map;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace GameUnitTests
{
    public class TestJoinWorld
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame(createPlayer: false);
            Serialization.LoadSerializers();
        }

        [Test]
        public void TestJoinCreatingPlayerData()
        {
            var playersBefore = Game.World.Players.PlayerCount;
            var joinEvent = new JoinWorldMapCommand();
            var clientPlayer = new TestServerPlayer(Game);
            Game.HandleClientEvent(clientPlayer, joinEvent);

            var createdPlayer = Game.World.Players.GetPlayer(clientPlayer.EntityId);

            Assert.AreEqual(1, Game.World.Players.PlayerCount);
            Assert.AreEqual(clientPlayer, createdPlayer);
            Assert.AreEqual(1, createdPlayer.EntityLogic.GetBuildings().Count);  // initial building
            Assert.IsTrue(createdPlayer.EntityLogic.GetParties()[0] != null);    // initial party
        }

        [Test]
        public void TestJoinInitialDataEventsSent()
        {
            var playersBefore = Game.World.Players.PlayerCount;
            var joinEvent = new JoinWorldMapCommand();
            var clientPlayer = new TestServerPlayer(Game);
            Game.HandleClientEvent(clientPlayer, joinEvent);

            var updateEvents = clientPlayer.ReceivedPacketsOfType<EntityUpdatePacket>().Where(e => e.Type != EntityType.Tile).ToList();
            var partyEvent = updateEvents.FirstOrDefault(e => e.Type == EntityType.Party);
            var buildingEvent = updateEvents.FirstOrDefault(e => e.Type == EntityType.Building);
            var partyPosition = (MapPlacementComponent)partyEvent.SyncedComponents.FirstOrDefault(c => c.GetType() == typeof(MapPlacementComponent));
            var buildingPosition = (MapPlacementComponent)buildingEvent.SyncedComponents.FirstOrDefault(c => c.GetType() == typeof(MapPlacementComponent));

            Assert.AreEqual(2, updateEvents.Count, "Initial Party & Building should be visible");
            Assert.AreNotEqual(partyPosition.Position.X, 0);
            Assert.AreNotEqual(partyPosition.Position.Y, 0);
            Assert.AreNotEqual(buildingPosition.Position.X, 0);
            Assert.AreNotEqual(buildingPosition.Position.Y, 0);
        }

        [Test]
        public void TestNewPlayerReceivingEvents()
        {
            var playersBefore = Game.World.Players.PlayerCount;
            var joinEvent = new JoinWorldMapCommand();
            var player = new TestServerPlayer(Game);

            var tile = player.Game.World.GetTile(0, 0);

            Game.HandleClientEvent(player, joinEvent);

            var entityUpdates = player.ReceivedPacketsOfType<EntityUpdatePacket>().Where(e => e.Type != EntityType.Tile).ToList();
            var tileUpdates = player.ReceivedEntityUpdates(EntityType.Tile);

            Assert.IsTrue(tileUpdates.Count > 2);
            Assert.AreEqual(2, entityUpdates.Count);
            Assert.IsTrue(entityUpdates.Where(e => e.EntityId == player.GetParty(0).EntityId).Any());
            Assert.IsTrue(entityUpdates.Where(e => e.EntityId == player.Buildings.First().EntityId).Any());
        }

        [Test]
        public void TestJoinNewPlayer()
        {
            var playersBefore = Game.World.Players.PlayerCount;
            var joinEvent = new JoinWorldMapCommand();
            var player = new TestServerPlayer(Game);
            Game.HandleClientEvent(player, joinEvent);

            var entityVisibleEvents = player.ReceivedPacketsOfType<EntityUpdatePacket>().Where(e => e.Type != EntityType.Tile).ToList();

            Assert.AreEqual(2, entityVisibleEvents.Count, "Should view his castle and his party");
        }

        [Test]
        public void TestJoinExistingPlayer()
        {
            var playersBefore = Game.World.Players.PlayerCount;
            var joinEvent = new JoinWorldMapCommand();
            var player = new TestServerPlayer(Game);

            Game.HandleClientEvent(player, joinEvent);

            var firstEntityVisibleEvents = player.ReceivedPacketsOfType<EntityUpdatePacket>();
            var firstTileVisibleEvents = player.ReceivedEntityUpdates(EntityType.Tile);
            var p1 = player.ReceivedEntityUpdates(EntityType.Party);

            player.ReceivedPackets.Clear();

            Game.HandleClientEvent(player, joinEvent);

            var secondEntityVisibleEvents = player.ReceivedPacketsOfType<EntityUpdatePacket>();
            var secondTileVisibleEvents = player.ReceivedEntityUpdates(EntityType.Tile);

            var p2 = player.ReceivedEntityUpdates(EntityType.Party);

            Assert.AreEqual(p1.Count, p2.Count);
            Assert.AreEqual(firstEntityVisibleEvents.Count, secondEntityVisibleEvents.Count);
            Assert.AreEqual(firstTileVisibleEvents.Count, secondTileVisibleEvents.Count);
        }

        [Test]
        public void TestInitialUnitStats()
        {
            var playersBefore = Game.World.Players.PlayerCount;
            var joinEvent = new JoinWorldMapCommand();
            var player = new TestServerPlayer(Game);
            Game.HandleClientEvent(player, joinEvent);
            var party = player.GetParty(0);
            var unit = party.Get<BattleGroupComponent>().Units[0];

            Assert.That(unit.HP == unit.MaxHP);
        }
    }
}