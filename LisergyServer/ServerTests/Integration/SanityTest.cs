using ClientSDK.Data;
using Game;
using Game.DataTypes;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Systems.Tile;
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
    /// This test will run server in a thread and client in another thread and do actual networking using local network.
    /// This is a single test that should cover the main use case of the client game SDK end to end
    /// This is done as a sdk instead of a client so it's easier to test and debug and ensure its logic is fully functional and can be safely
    /// exposed to client so the client is only required to ensure the UI and visual representations are working.
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
        public void TearDown() => _server?.Dispose();

        [Test]

        public async Task SmokeTestFlow()
        {
            // PREPARE SDK
            GameId.DEBUG_MODE = 1;
            var mapPlacementUpdates = new List<IEntity>();
            _client.Modules.Components.OnComponentUpdate<MapPlacementComponent>((e, oldValue, newValue) =>
            {
                mapPlacementUpdates.Add(e);
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
            Assert.AreEqual(3, mapPlacementUpdates.Count); // Castle, Party, Dungeon
            Assert.AreEqual(1, mapPlacementUpdates.Count(c => c is PartyEntity));
            Assert.AreEqual(1, mapPlacementUpdates.Count(c => c is PlayerBuildingEntity));
            Assert.AreEqual(1, mapPlacementUpdates.Count(c => c is DungeonEntity));
            mapPlacementUpdates.Clear();

            // CHECK IF CLIENT-SIDE EVENTS ARE FIRED
            Assert.Greater(_client.EventsInClientLogic.Count(e => e is TileVisibilityChangedForPlayerEvent), 0);

            // UPDATE ENTITY COMPONENTS
            foreach (var packet in _client.ReceivedPackets.Where(p => p is EntityUpdatePacket update && update.Type == EntityType.Party))
            {
                var partyUpdatePacket = (EntityUpdatePacket)packet;
                var clientParty = (PartyEntity)_client.Game.Entities[partyUpdatePacket.EntityId];

                var placement = clientParty.Get<MapPlacementComponent>();
                Assert.That(placement.Position.X > 0 && placement.Position.Y > 0);
            }

            // MOVEMENT REQUEST
            var party = _client.Modules.Player.LocalPlayer.GetParty(0);
            var originalTile = party.Tile;
            var nextTile = originalTile.GetNeighbor(Direction.SOUTH);
            Assert.IsTrue(_client.Modules.Actions.MoveParty(party, nextTile, CourseIntent.Defensive));

            // DISCONNECT & RECONNECT
            Log.Debug("----- RECONNECTING -----");
            _client.Network._socket.Disconnect();
            _client.ReceivedPackets.Clear();
            _client.Network.Tick();

            // RE-LOGIN
            _client.Modules.Account.SendAuthenticationPacket("abc", "def");
            result = await _client.WaitFor<AuthResultPacket>();
            Assert.AreEqual(true, result.Success);

            // CHECK COMPONENTS SYNCED
            await _client.WaitFor<EntityUpdatePacket>(e => e.Type == EntityType.Party);
            await _client.WaitFor<EntityUpdatePacket>(e => e.Type == EntityType.Dungeon);
            await _client.WaitFor<EntityUpdatePacket>(e => e.Type == EntityType.Building);

            Assert.AreEqual(3, mapPlacementUpdates.Count); // Castle, Party, Dungeon
            Assert.AreEqual(1, mapPlacementUpdates.Count(c => c is PartyEntity));
            Assert.AreEqual(1, mapPlacementUpdates.Count(c => c is PlayerBuildingEntity));
            Assert.AreEqual(1, mapPlacementUpdates.Count(c => c is DungeonEntity));
        }
    }
}
