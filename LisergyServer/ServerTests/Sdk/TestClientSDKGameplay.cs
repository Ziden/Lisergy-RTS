using Game;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Events.ServerEvents;
using Game.Systems.Map;
using Game.Systems.Player;
using Game.Systems.Resources;
using Game.World;
using GameDataTest;
using GameDataTest.TestWorldGenerator;
using NUnit.Framework;
using ServerTests.Integration.Stubs;
using System.Linq;
using System.Threading.Tasks;
using Tests.Unit.Stubs;

namespace SdkUnitTests
{
    public class TestClientSDKGameplay
    {
        TestGameClient _client;
        GameId _playerId;
        LisergyGame _serverLogic;
        PlayerModel _serverPlayer;

        [SetUp]
        public void Setup()
        {
            _client = new TestGameClient(null);
            _playerId = GameId.Generate();
            var specs = TestSpecs.Generate();
            _serverLogic = new LisergyGame(specs, new GameLog("[Server]"));
            _serverLogic.SetupWorld(new TestWorld(_serverLogic));
            _serverLogic.Network.ReceiveInput(_playerId, new JoinWorldMapCommand());
            _serverPlayer = _serverLogic.Players[_playerId];
            _client.Network.ReceiveInput(_playerId, new LoginResultPacket()
            {
                Profile = new PlayerProfileComponent(_playerId),
                Success = true
            });
            _client.Network.ReceiveInput(_playerId, new GameSpecPacket(_serverLogic));
            foreach (var loc in _serverLogic.Players[_playerId].EntityLogic.GetVisibleTiles())
            {
                _client.Network.ReceiveInput(
                    _playerId, _serverLogic.World.GetTile(loc).Entity.Logic.DeltaCompression.GetUpdatePacket(_playerId, false));
            }
            _client.Network.ReceiveInput(
                _playerId, _serverPlayer.EntityLogic.GetParties()[0].Logic.DeltaCompression.GetUpdatePacket(_playerId, false));
            _client.EventsInSdk.Clear();
            _client.EventsInClientLogic.Clear();
            _client.ReceivedPackets.Clear();
        }

        [Test]
        public void TestResourcesOnTiles()
        {
            var map = _client.Game.World as GameWorld;
            foreach (var tile in map.AllTiles())
            {
                if (tile != null && tile.Components.Has<TileResourceComponent>() && !tile.HasHarvestSpot)
                {
                    Assert.Fail($"Tile {tile} has invalid resource ?");
                }
            }
        }

        [Test]
        public async Task TestHarvestingSync()
        {
            var map = _client.Game.World as GameWorld;
            var clientTile = map.AllTiles().Where(t => t != null && t.Components.Has<TileResourceComponent>()).First();
            var serverTile = _serverLogic.World.GetTile(clientTile.Position);
            var serverParty = _serverLogic.Players[_playerId].EntityLogic.GetParties()[0];
            var clientParty = _client.Modules.Player.LocalPlayer.EntityLogic.GetParties()[0];

            serverParty.Logic.Map.SetPosition(serverTile);
            serverParty.Logic.Harvesting.StartHarvesting(serverTile);

            var party = _serverPlayer.EntityLogic.GetParties()[0];
            var tile = party.GetTile();

            var tilePacket = tile.Logic.DeltaCompression.GetUpdatePacket(_playerId, false);
            var partyPacket = party.Logic.DeltaCompression.GetUpdatePacket(_playerId, false);

            _client.Network.ReceiveInput(_playerId, tilePacket);
            _client.Network.ReceiveInput(_playerId, partyPacket);

            Assert.IsTrue(clientParty.Components.Has<HarvestingComponent>());
            Assert.IsTrue(clientParty.GetTile().Components.Get<TileResourceComponent>().BeingHarvested);
        }
    }
}
