using Game.Engine.ECLS;
using Game.Entities;
using Game.Events.ServerEvents;
using Game.Systems.Map;
using Game.Systems.Resources;
using Game.Systems.Tile;
using Game.Tile;
using GameDataTest;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace GameUnitTests
{
    public class TestResources
    {
        private TestGame _game;
        private IEntity _party;
        private TileModel _logs;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame(createPlayer: false);
            _game.World.GetTile(2, 2).Logic.Tile.SetTileId(TestTiles.FOREST.ID);
        }

        [Test]
        public void TestTileResourcesInitialSetup()
        {
            var world = _game.TestWorld;
            var joinEvent = new JoinWorldMapCommand();
            var clientPlayer = new TestServerPlayer(_game);
            _game.HandleClientEvent(clientPlayer, joinEvent);
            var party = clientPlayer.GetParty(0);

            foreach (var tilePosition in clientPlayer.VisibilityData.VisibleTiles)
            {
                var tile = world.GetTile(tilePosition.X, tilePosition.Y);
                if (tile.Components.Has<TileResourceComponent>())
                {
                    if (!tile.HasHarvestSpot) Assert.Fail($"Tile {tile} has invalid resource ?");
                    var packet = clientPlayer.ReceivedPackets.First(p => p is EntityUpdatePacket up && up.Type == EntityType.Tile && up.GetComponent<TileDataComponent>().Position == tile.Position) as EntityUpdatePacket;
                    Assert.That(packet.SyncedComponents.Any(c => c.GetType() == typeof(TileResourceComponent)));
                }
            }
        }
    }
}