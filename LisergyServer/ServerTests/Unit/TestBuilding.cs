using Game;
using Game.World;
using Game.World.Components;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace Tests
{
    public class TestBuilding
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame();
        }

        [Test]
        public void TestInitialBuilding()
        {
            var player = Game.GetTestPlayer();
 
            var initialBuildingSpec = StrategyGame.Specs.Buildings[StrategyGame.Specs.InitialBuilding];
            var building = player.Buildings.FirstOrDefault();
            var tile = building.Tile;

            Assert.IsTrue(player.Buildings.Count == 1);
            Assert.IsTrue(player.Buildings.Any(b => b.SpecID == initialBuildingSpec.Id));
            Assert.IsTrue(tile.GetComponent<EntityPlacementComponent>().StaticEntity == player.Buildings.First());
            Assert.IsTrue(((Building)tile.GetComponent<EntityPlacementComponent>().StaticEntity).SpecID == initialBuildingSpec.Id);
        }

        [Test]
        public void TestNewBuilding()
        {
            var player = Game.GetTestPlayer();
            var initialBuildingSpec = TestGame.RandomBuildingSpec();
            var tile = Game.RandomNotBuiltTile();
            var buildingSpec = TestGame.RandomBuildingSpec();

            player.Build(buildingSpec.Id, tile);

            Assert.IsTrue(player.Buildings.Count == 2);
            Assert.IsTrue(player.Buildings.Any(b => b.SpecID == buildingSpec.Id));
            Assert.IsTrue(tile.GetComponent<EntityPlacementComponent>().StaticEntity == player.Buildings.Last());
            Assert.IsTrue(((Building)tile.GetComponent<EntityPlacementComponent>().StaticEntity).SpecID == buildingSpec.Id);

            var nei = tile.GetNeighbor(Direction.NORTH);
            Assert.That(tile.GetComponent<TileVisibilityComponent>().EntitiesViewing.Contains(tile.GetComponent<EntityPlacementComponent>().StaticEntity));

        }
    }
}