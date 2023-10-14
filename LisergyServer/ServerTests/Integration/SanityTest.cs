using ClientSDK.Data;
using Game;
using Game.Events.ServerEvents;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.Map;
using Game.Systems.Party;
using Game.Tile;
using Game.World;
using NUnit.Framework;
using ServerTests.Integration.Stubs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerTests.Integration
{
    /// <summary>
    /// This test will run server in a thread and client in another thread and do actual networking
    /// </summary>
    public class TestClientSDKSmoke
    {
        TestServerThread _server;
        TestGameClient _client;

        [SetUp]
        public void Setup()
        {
            _server = new TestServerThread();
            _client = new TestGameClient();
        }

        [TearDown]
        public void TearDown() => _server.Dispose();

        [Test]
        public async Task SmokeTestFlow()
        {
            // PREPARE SDK
            var partyPlacementUpdates = new List<MapPlacementComponent>();
            _client.Modules.Components.OnComponentUpdate<PartyEntity, MapPlacementComponent>((e, c) =>
            {
                partyPlacementUpdates.Add(c);
            });

            _client.Modules.Views.RegisterView<TileEntity, EntityView<TileEntity>>();
            _client.Modules.Views.RegisterView<PartyEntity, EntityView<PartyEntity>>();
            _client.Modules.Views.RegisterView<DungeonEntity, EntityView<DungeonEntity>>();
            _client.Modules.Views.RegisterView<PlayerBuildingEntity, EntityView<PlayerBuildingEntity>>();

            // LOGIN
            _client.Modules.Account.SendAuthenticationPacket("abc", "def");
            var result = await _client.WaitFor<AuthResultPacket>();
            Assert.AreEqual(true, result.Success);

            // RECEIVE TILES
            var firstReceived = await _client.WaitFor<TilePacket>();
            Assert.NotNull(firstReceived);
            foreach (var tileUpdate in _client.ReceivedPackets.Where(p => p.GetType() == typeof(TilePacket)))
            {
                var tilePacket = (TilePacket)tileUpdate;
                var tile = _client.Game.World.Map.GetTile(tilePacket.Data.X, tilePacket.Data.Y);
                Assert.AreEqual(tile.X, tilePacket.Data.X);
                Assert.AreEqual(tile.Y, tilePacket.Data.Y);
                Assert.AreEqual(tile.SpecId, tilePacket.Data.TileId);
                Assert.AreEqual(new Position(tile.X >> GameWorld.CHUNK_SIZE_BITSHIFT, tile.Y >> GameWorld.CHUNK_SIZE_BITSHIFT), tile.Chunk.Position);
            }

            // RECEIVE ENTITIES
            var firstEntityReceived = await _client.WaitFor<EntityUpdatePacket>();
            Assert.NotNull(firstEntityReceived);
            foreach (var packet in _client.ReceivedPackets.Where(p => p.GetType() == typeof(EntityUpdatePacket)))
            {
                var entityUpdate = (EntityUpdatePacket)packet;
                var clientEntity = _client.Game.Entities[entityUpdate.EntityId];
                Assert.NotNull(clientEntity);
                Assert.AreEqual(clientEntity.EntityId, entityUpdate.EntityId);
                Assert.AreEqual(clientEntity.OwnerID, entityUpdate.OwnerId);

                // Entitty View was created
                Assert.NotNull(_client.Modules.Views.GetEntityView(clientEntity));
            }

            // COMPONENTS SYNC TRIGGERED
            await _client.WaitFor<EntityUpdatePacket>(p => p.Type == EntityType.Party);
            Assert.AreEqual(1, partyPlacementUpdates.Count);

            // UPDATE ENTITY COMPONENTS
            foreach (var packet in _client.ReceivedPackets.Where(p => p is EntityUpdatePacket update && update.Type == EntityType.Party))
            {
                var partyUpdatePacket = (EntityUpdatePacket)packet;
                var clientParty = (PartyEntity)_client.Game.Entities[partyUpdatePacket.EntityId];

                var placement = clientParty.Get<MapPlacementComponent>();
                Assert.That(placement.Position.X > 0 && placement.Position.Y > 0);
            }
        }
    }
}
