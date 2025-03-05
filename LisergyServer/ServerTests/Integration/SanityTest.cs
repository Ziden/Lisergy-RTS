using ClientSDK.SDKEvents;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Entities;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using Game.Systems.FogOfWar;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.Systems.Resources;
using Game.Systems.Tile;
using Game.Tile;
using Game.World;
using NUnit.Framework;
using ServerTests;
using ServerTests.Integration.Stubs;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tests.Unit.Stubs;

namespace SmokeTests
{
    /// <summary>
    /// This test will run server in a thread and client in another thread and do actual networking using local network.
    /// This is a single test that should cover the main use case of the client game SDK end to end
    /// This is done as a sdk instead of a client so it's easier to test and debug and ensure its logic is fully functional and can be safely
    /// exposed to client so the client is only required to ensure the UI and visual representations are working.
    /// </summary>
    public class SmokeTest
    {
        StandaloneServer _server;
        TestGameClient _client;
        GameId _playerId;

        [OneTimeSetUp]
        public async Task Setup()
        {
            GameId.INCREMENTAL_MODE = 1;
            _server = new StandaloneServer();
            _server.Multithreaded = false;
            _server.Start();
            _client = new TestGameClient(_server);
            await _client.PrepareSDK();
        }

        [OneTimeTearDown]
        public void TearDown() => _server?.Dispose();

        [Test]
        [NonParallelizable]
        public async Task SmokeTestFlow()
        {
            await Disconnect();
            await Login();

            Assert.NotNull(_client.Game.Players[_playerId]);
            Assert.AreEqual(_client.Modules.Player.LocalPlayer, _client.Game.Players[_playerId]);

            Assert.IsFalse(_client.Game.Network.DeltaCompression.Enabled);
            Assert.IsTrue(_server.Game.Network.DeltaCompression.Enabled);

            await ValidateMapTiles();
            await ValidateEntities();
            await ValidateMapResources();

            var party = _client.Modules.Player.LocalPlayer.EntityLogic.GetParties()[0];
            var resourceTile = await ValidateHarvestingLocation();

            await MoveEntityTo(party, resourceTile, CourseIntent.Harvest);

            await ValidateIsHarvesting(party, resourceTile);

            await Task.Delay(1000); // 1 second of harvest

            await StopHarvesting(party);

            // Disconnecting the player
            await Disconnect();

            // Hard-reconnect
            _client = new TestGameClient(_server);
            await _client.PrepareSDK();
            await Login();

            await ValidateMapTiles();
            await ValidateEntities();
        }

        private async Task Disconnect()
        {
            // Disconnected before receiving
            _client.Modules.Account.SendAuthenticationPacket("abc", "def");
            _server.SingleThreadTick();
            _client.Network.Disconnect();
            var result = await _client.WaitUntilReceives<LoginResultPacket>();
            Assert.IsNull(result);
        }

        private async Task Login()
        {
            // LOGIN, will reconnect automatically if disconnected
            _client.Modules.Account.SendAuthenticationPacket("abc", "def");
            await _client.WaitUntilSends<LoginPacket>();
            var result = await _client.WaitUntilReceives<LoginResultPacket>();
            Assert.AreEqual(true, result.Success);
            _client.ClearEvents();
            await _client.WaitUntilSends<JoinWorldMapCommand>();
            _playerId = _client.Modules.Player.LocalPlayer.EntityId;
            var serverVisibleTiles = _server.Game.Players[_playerId]?.EntityLogic.GetVisibleTiles().Count ?? 42;
            await _client.WaitUntilPacketCount<EntityUpdatePacket>(serverVisibleTiles);
        }

        private async Task<TileModel> ValidateHarvestingLocation()
        {
            _client.ClearEvents();
            var party = _client.Game.Entities.GetChildren(_client.Modules.Player.LocalPlayer.EntityId, EntityType.Party).First();
            TileModel resourceTile = null;
            var distance = 0;
            foreach (var tile in party.GetTile().GetAOE(2))
            {
                if (tile == null || !tile.Components.Get<TileVisibilityComponent>().PlayersViewing.Contains(_playerId))
                {
                    continue;
                }
                var d = party.GetTile().Distance(tile);
                if (tile.HasHarvestSpot && (resourceTile == null || d < distance))
                {
                    resourceTile = tile;
                    distance = d;
                }
            }
            Assert.NotNull(resourceTile);
            return resourceTile;
        }

        private async Task ValidateMapTiles()
        {
            var visibleTiles = _client.Modules.Player.LocalPlayer.EntityLogic.GetVisibleTiles();
            var firstReceived = await _client.WaitUntilReceives<EntityUpdatePacket>(p => p.Type == EntityType.Tile);
            var firstEntityReceived = await _client.WaitUntilReceives<EntityUpdatePacket>(p => p.Type == EntityType.Party);
            var tileUpdatesReceived = _client.ReceivedPackets.Where(p => p is EntityUpdatePacket e && e.Type == EntityType.Tile).Cast<EntityUpdatePacket>();
            var visibilityEvents = _client.FilterClientEvents<TileVisibilityChangedEvent>();
            var entityUpdates = _client.FilterReceivedPackets<EntityUpdatePacket>();
            var tileViewsCreated = _client.EventsInSdk.Count(e => e is EntityViewRendered c && c.Entity.EntityType == EntityType.Tile);
            var tileVisibleEvents = _client.EventsInClientLogic.Count(e => e is TileVisibilityChangedEvent);
            Assert.NotNull(firstReceived);
            Assert.NotNull(firstEntityReceived);

            foreach (var tileUpdate in tileUpdatesReceived)
            {
                var tileData = tileUpdate.GetComponent<TileDataComponent>();

                var clientTile = _client.Game.World.GetTile(tileData.Position);
                var serverTile = _server.Game.World.GetTile(tileData.Position);

                Assert.IsTrue(clientTile.Components.IsUpToDateWith(serverTile.Entity));
                Assert.AreEqual(clientTile.X, tileData.Position.X);
                Assert.AreEqual(clientTile.Y, tileData.Position.Y);
                Assert.AreEqual((byte)clientTile.SpecId, tileData.TileId);
                Assert.AreEqual(new Location(clientTile.X >> GameWorld.CHUNK_SIZE_BITSHIFT, clientTile.Y >> GameWorld.CHUNK_SIZE_BITSHIFT), clientTile.Chunk.Position);
                Assert.AreEqual(clientTile.EntityId, tileUpdate.EntityId);
                Assert.AreEqual(serverTile.EntityId, clientTile.EntityId);
            }

            Assert.Greater(visibleTiles.Count, 20);
            Assert.AreEqual(visibleTiles.Count, visibilityEvents.Count);
            Assert.AreEqual(tileUpdatesReceived.Count(), visibleTiles.Count);
            Assert.NotNull(firstEntityReceived);
            Assert.Greater(tileViewsCreated, 20);
            Assert.Greater(tileVisibleEvents, 20);
            Assert.AreEqual(tileViewsCreated, tileVisibleEvents);
        }

        private async Task ValidateEntities()
        {
            var entityUpdates = _client.FilterReceivedPackets<EntityUpdatePacket>();
            foreach (var entityUpdate in entityUpdates.Where(u => u.Type != EntityType.Tile))
            {
                var clientEntity = _client.Game.Entities[entityUpdate.EntityId];
                var serverEntity = _server.Game.Entities[entityUpdate.EntityId];

                Assert.IsTrue(clientEntity.Components.IsUpToDateWith(serverEntity));
                Assert.NotNull(clientEntity);
                Assert.AreEqual(clientEntity.EntityId, entityUpdate.EntityId);
                Assert.AreEqual(clientEntity.EntityId, serverEntity.EntityId);
                Assert.AreEqual(clientEntity.OwnerID, entityUpdate.OwnerId);
                Assert.NotNull(_client.Modules.Views.GetEntityView(clientEntity));

                var clientPlacement = clientEntity.Get<MapPlacementComponent>().Position;
                var serverPlacement = serverEntity.Get<MapPlacementComponent>().Position;

                Assert.AreEqual(clientPlacement, serverPlacement);
            }

            foreach (var serverEntity in _server.Game.Entities.AllEntities)
            {
                if (serverEntity.EntityType == EntityType.Player) continue;

                if (!serverEntity.Logic.Vision.IsVisibleFor(_server.Game.Players[_playerId].PlayerEntity))
                {
                    Assert.IsNull(_client.Game.Entities[serverEntity.EntityId]);
                }
                else
                {
                    Assert.NotNull(_client.Game.Entities[serverEntity.EntityId]);

                    var clientEntity = _client.Game.Entities[serverEntity.EntityId];
                    var serverTile = serverEntity.GetTile();
                    var clientTile = clientEntity.GetTile();
                    if (serverEntity.EntityType == EntityType.Party)
                    {
                        Assert.IsTrue(clientTile.Logic.Tile.GetEntitiesOnTile().Contains(clientEntity));
                    }
                    else if (serverEntity.EntityType == EntityType.Building)
                    {
                        Assert.AreEqual(clientEntity, clientTile.Logic.Tile.GetBuildingOnTile());
                    }

                    Assert.AreEqual(clientEntity.Get<MapPlacementComponent>()?.Position, serverEntity.Get<MapPlacementComponent>()?.Position);

                }
            }
            TestGame.ValidateNoLeak(_server.Game);
        }


        private async Task ValidateIsHarvesting(IEntity party, TileModel resourceTile)
        {
            // Check i received the update
            var entityUpdate = await _client.WaitUntilReceives<EntityUpdatePacket>(p => p.SyncedComponents.Any(c => c.GetType() == typeof(HarvestingComponent)));
            var syncedHarvest = (HarvestingComponent)entityUpdate.SyncedComponents.First(c => c.GetType() == typeof(HarvestingComponent));
            Assert.That(syncedHarvest.StartedAt != 0);

            var tileUpdate = await _client.WaitUntilReceives<EntityUpdatePacket>(p => p.EntityId == resourceTile.EntityId);
            var tileUpdates = _client.FilterReceivedPackets<EntityUpdatePacket>().Where(p => p.EntityId == resourceTile.EntityId).ToList();

            // check update client logic
            Assert.AreEqual(syncedHarvest.StartedAt, party.Get<HarvestingComponent>().StartedAt);
            Assert.AreEqual(resourceTile.Position, party.Get<HarvestingComponent>().Tile);
            Assert.AreEqual(resourceTile.EntityId, party.GetTile().EntityId);

            var t2 = _client.Game.World.GetTile(resourceTile.Position);
            Assert.IsTrue(resourceTile.Get<TileResourceComponent>().BeingHarvested);
        }

        private async Task MoveEntityTo(IEntity party, TileModel resourceTile, CourseIntent intent)
        {
            _client.ClearEvents();
            var expectedMovementRange = resourceTile.Distance(party.Logic.Map.GetTile());
            Assert.IsTrue(_client.Modules.Actions.MoveEntity(party, resourceTile, intent));

            // Course id updating correctly
            var moveComponent = await _client.WaitForEntityComponentUpdate<MovementComponent>(party);
            Assert.AreNotEqual(GameId.ZERO, moveComponent.CourseId);

            while (party.GetTile() != resourceTile)
            {
                await _client.WaitUntilReceives<EntityUpdatePacket>();
            }

            Assert.That(party.Logic.Map.GetTile() == resourceTile);
            Assert.That(party.Get<MapPlacementComponent>().Position == resourceTile.Position);

            var moveEvents = _client.FilterClientEvents<ComponentUpdateEvent<MapPlacementComponent>>();
            Assert.AreEqual(expectedMovementRange, moveEvents.Count());

            moveComponent = await _client.WaitForEntityComponentUpdate<MovementComponent>(party);
            Assert.AreEqual(GameId.ZERO, moveComponent.CourseId);

        }

        private async Task StopHarvesting(IEntity party)
        {
            _client.ReceivedPackets.Clear();
            Assert.IsTrue(_client.Modules.Actions.StopEntity(party));
            var update = await _client.WaitUntilReceives<EntityUpdatePacket>(p => p.EntityId == party.EntityId);

            // Server should have remoevd the Harvesting Component
            Assert.IsFalse(update.SyncedComponents.Any(c => c.GetType() == typeof(HarvestingComponent)));
            Assert.IsTrue(update.RemovedComponentIds.Any(c => c == Serialization.GetTypeId(typeof(HarvestingComponent))));

            // Client needs to have also remoevd the harvesting component
            Assert.IsFalse(_client.Game.Entities[party.EntityId].Components.Has<HarvestingComponent>());

            // Check i collected some resources and my cargo was updated on client
            var cargo = party.Get<CargoComponent>();
            Assert.That(cargo.Slot1.Amount > 0);
        }

        private async Task ValidateMapResources()
        {
            // VALIDATE TILE RESOURCES SYNCED
            foreach (var tile in ((GameWorld)_client.Game.World).AllTiles().Where(t => t != null))
            {
                if ((tile.Components.Has<TileResourceComponent>() && !tile.HasHarvestSpot) || (!tile.Components.Has<TileResourceComponent>() && tile.HasHarvestSpot))
                {
                    Assert.Fail($"Tile {tile} has invalid resource ?");
                }
            }

        }
    }
}
