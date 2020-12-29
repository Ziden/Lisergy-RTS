using Game;
using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace Tests
{
    public class TestLineOfSight
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame();
        }

        [Test]
        public void TestLOSEvents()
        {
            var initialBuildingSpec = StrategyGame.Specs.GetBuildingSpec(StrategyGame.Specs.InitialBuilding);
            var player = Game.GetTestPlayer();
            var events = Game.ReceivedEvents
                .Where(e => e is TileVisibleEvent)
                .Select(e => (TileVisibleEvent)e)
                .Where(e => e.Viewer.Owner == player)
                .ToList();

            var range = initialBuildingSpec.LOS * 2 + 1;
            Assert.That(events.Count == range * range);
        }

        [Test]
        public void TestInitialLOS()
        {
            var player = Game.GetTestPlayer();
 
            var initialBuildingSpec = StrategyGame.Specs.GetBuildingSpec(StrategyGame.Specs.InitialBuilding);
            var building = player.Buildings.FirstOrDefault();
            var tile = building.Tile;
            var areaTiles = tile.GetAOE(initialBuildingSpec.LOS).ToList();

            Assert.AreEqual(player.VisibleTiles.Count, areaTiles.Count());

            foreach (var seenTile in areaTiles)
            {
                Assert.True(seenTile.IsVisibleTo(player));
                Assert.True(seenTile.Viewing.Contains(building));
                Assert.True(player.VisibleTiles.Contains(seenTile));
            }
        }   

        [Test]
        public void TestTileAOE()
        {
            var tile = Game.World.GetTile(12, 12);

            ushort aoe = 4;
            var range = aoe * 2 + 1;
            var los = tile.GetAOE(aoe).ToList();

            Assert.That(los.Count() == range * range);
        }

        [Test]
        public void TestNewLosMiddleMap()
        {
            var initialBuildingSpec = StrategyGame.Specs.GetBuildingSpec(StrategyGame.Specs.InitialBuilding);
            var player = Game.GetTestPlayer();
            Game.ReceivedEvents.Clear();
        }
    }
}