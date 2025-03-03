using ClientSDK.Data;
using ClientSDK.SDKEvents;
using Game;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Entities;
using Game.Events.ServerEvents;
using Game.Systems.FogOfWar;
using Game.Systems.Map;
using Game.Systems.Player;
using Game.Systems.Tile;
using GameDataTest;
using GameDataTest.TestWorldGenerator;
using NUnit.Framework;
using ServerTests.Integration.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Integration.Stubs;

namespace UnitTests
{
    public class TestGameClientSDKSetup
    {
        TestGameClient _client;
        GameId _playerId;
        LisergyGame _serverLogic;
        PlayerModel _serverPlayer;

        private EntityView OnCreateView(IEntity e)
        {
            if (e.EntityType == EntityType.Tile) return new StubEntityView(e, _client);
            return new EntityView(e, _client);
        }

        [SetUp]
        public void Setup()
        {
            _client = new TestGameClient(null);
            _client.Modules.Views.CreatorFunction = OnCreateView;
            _playerId = GameId.Generate();
            var specs = TestSpecs.Generate();
            var serverLog = new GameLog("[Server]");
            serverLog._Debug = m => { };
            _serverLogic = new LisergyGame(specs, serverLog);
            _serverLogic.SetupWorld(new TestWorld(_serverLogic));
            _serverLogic.Network.ReceiveInput(_playerId, new JoinWorldMapCommand());
            _serverPlayer = _serverLogic.Players[_playerId];
            _client.Network.ReceiveInput(_playerId, new LoginResultPacket()
            {
                Profile = new PlayerProfileComponent(_playerId),
                Success = true
            });
            _client.Network.ReceiveInput(_playerId, new GameSpecPacket(_serverLogic));
        }

        public IEnumerable<IComponent> FindSyncDifferences(IEntity one, IEntity two)
        {
            foreach (var kp in one.Components.GetComponents())
            {
                var c = two.Components.GetComponents()[kp.Key];
                if (!c.Equals(kp.Value)) yield return kp.Value;
            }
        }

        [Test]
        public void TestGameWasCreated()
        {
            // Ensuring SDK initialized game
            var clientLocalPlayer = _client.Game.Players[_playerId];
            var clientPlayerEntity = _client.Game.Entities[_playerId];
            Assert.NotNull(clientLocalPlayer);
            Assert.NotNull(clientPlayerEntity);
            Assert.NotNull(_client.Game.World);
            Assert.NotNull(_client.Game.Specs);

            foreach (var clientEntity in _client.Game.Entities.AllEntities)
            {
                var serverEntity = _serverLogic.Entities[clientEntity.EntityId];

                Assert.IsTrue(clientEntity.Components.IsUpToDateWith(serverEntity));
            }
        }

        [Test]
        public void TestTilesBeingSent()
        {
            // Sending tiles
            var tilesVisibleOnServer = _serverLogic.Players[_playerId].EntityLogic.GetVisibleTiles();
            foreach (var loc in tilesVisibleOnServer)
            {
                var tile = _serverLogic.World.GetTile(loc);
                _client.Network.ReceiveInput(
                    _playerId, tile.TileEntity.Logic.DeltaCompression.GetUpdatePacket(_playerId, false));
            }

            // Ensure sdk created views for those tiles
            Assert.AreEqual(tilesVisibleOnServer.Count, _client.FilterClientEvents<EntityViewCreated>().Where(e => e.Entity.EntityType == Game.Entities.EntityType.Tile).Count());
            foreach (var loc in tilesVisibleOnServer)
            {
                var serverTile = _serverLogic.World.GetTile(loc);
                var clientTile = _client.Game.World.GetTile(loc);

                Assert.IsTrue(_client.Modules.Views.GetEntityView(clientTile.TileEntity) is StubEntityView);
                Assert.IsTrue(clientTile.TileEntity.Components.IsUpToDateWith(serverTile.TileEntity));
                Assert.IsTrue(serverTile.Components.CompareWith<TileDataComponent>(clientTile.TileEntity));
                Assert.AreEqual(serverTile.EntityId, clientTile.EntityId);
            }
        }

        [Test]
        public void TestEntityExploring()
        {
            // Creating client tiles
            var visibleTiles = _serverLogic.Players[_playerId].EntityLogic.GetVisibleTiles();
            foreach (var loc in visibleTiles)
            {
                _client.Network.ReceiveInput(
                    _playerId, _serverLogic.World.GetTile(loc).TileEntity.Logic.DeltaCompression.GetUpdatePacket(_playerId, false));
            }

            var serverParty = _serverPlayer.EntityLogic.GetParties()[0];

            // Sending my party
            _client.Network.ReceiveInput(
                _playerId, serverParty.Logic.DeltaCompression.GetUpdatePacket(_playerId, false));

            // did client placed the party ?
            var clientParty = _client.Game.Entities[serverParty.EntityId];

            // entity move event triggered on client by LogicModule ?  
            var visibilityEvents = _client.FilterClientEvents<TileVisibilityChangedEvent>();

            Assert.AreEqual(visibleTiles.Count, visibilityEvents.Count);
            Assert.AreEqual(clientParty.Get<MapPlacementComponent>().Position, serverParty.Get<MapPlacementComponent>().Position);

            // Client predicted all visibility and fog of war in LogicModule
            foreach (var loc in visibleTiles)
            {
                var clientTile = _client.Game.World.GetTile(loc);

                var entitiesViewing = clientTile.Logic.Vision.GetEntitiesViewing();
                var playersViewing = clientTile.Logic.Vision.GetPlayersViewing();

                Assert.IsTrue(entitiesViewing.Contains(serverParty.EntityId));
                Assert.IsTrue(playersViewing.Contains(_playerId));
            }
        }
    }
}
