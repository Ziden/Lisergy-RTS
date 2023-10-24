using Game;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Systems.Tile;
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
    [NonParallelizable]
    public class TestClientSDKSmoke
    {
        StandaloneServer _server;
        TestGameClient _client;

        [SetUp]
        public void Setup()
        {
            _server = new StandaloneServer().Start();
            _client = new TestGameClient();
            _client.PrepareSDK();
        }

        [TearDown]
        public void TearDown() => _server?.Dispose();

        [Test]
        [NonParallelizable]
        public async Task TestDisconnection()
        {
            _client.Modules.Account.SendAuthenticationPacket("abc", "def");
            _client.Network.Disconnect();
            var result = await _client.WaitFor<LoginResultPacket>();
            Assert.IsNull(result);
        }

        [Test]
        [NonParallelizable]
        public async Task TestReconnection()
        {
            var mapPlacementUpdates = new List<IEntity>();
            _client.Modules.Components.OnComponentUpdate<MapPlacementComponent>((e, oldValue, newValue) =>
            {
                mapPlacementUpdates.Add(e);
            });
            _client.Modules.Account.SendAuthenticationPacket("abc", "def");
            var result = await _client.WaitFor<LoginResultPacket>();
            Assert.NotNull(result);

            _client.Network.Disconnect();
            _client.ReceivedPackets.Clear();
            _client.Network.Tick();
            mapPlacementUpdates.Clear();

            _client.Modules.Account.SendAuthenticationPacket("abc", "def");
            result = await _client.WaitFor<LoginResultPacket>();
            Assert.AreEqual(true, result.Success);

            await _client.WaitFor<EntityUpdatePacket>(e => e.Type == EntityType.Party);
            await _client.WaitFor<EntityUpdatePacket>(e => e.Type == EntityType.Dungeon);
            await _client.WaitFor<EntityUpdatePacket>(e => e.Type == EntityType.Building);

            Assert.AreEqual(3, mapPlacementUpdates.Count); // Castle, Party, Dungeon
            Assert.AreEqual(1, mapPlacementUpdates.Count(c => c is PartyEntity));
            Assert.AreEqual(1, mapPlacementUpdates.Count(c => c is PlayerBuildingEntity));
            Assert.AreEqual(1, mapPlacementUpdates.Count(c => c is DungeonEntity));
        }

        [Test]
        [NonParallelizable]
        public async Task SmokeTestFlow()
        {        
            var mapPlacementUpdates = new List<IEntity>();
            _client.Modules.Components.OnComponentUpdate<MapPlacementComponent>((e, oldValue, newValue) =>
            {
                mapPlacementUpdates.Add(e);
            });

            // LOGIN
            _client.Modules.Account.SendAuthenticationPacket("abc", "def");
            var result = await _client.WaitFor<LoginResultPacket>();
            Assert.AreEqual(true, result.Success);

            // RECEIVE TILES
            var firstReceived = await _client.WaitFor<TilePacket>();
            Assert.NotNull(firstReceived);
            foreach (var tileUpdate in _client.ReceivedPackets.Where(p => p.GetType() == typeof(TilePacket)))
            {
                var tilePacket = (TilePacket)tileUpdate;
                var tile = _client.Game.World.Map.GetTile(tilePacket.Position.X, tilePacket.Position.Y);
                Assert.AreEqual(tile.X, tilePacket.Position.X);
                Assert.AreEqual(tile.Y, tilePacket.Position.Y);
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
        }
    }
}
