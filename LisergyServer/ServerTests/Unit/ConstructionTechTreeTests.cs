using Game.Systems.Building;
using Game.Systems.Player;
using GameData;
using NUnit.Framework;
using Game.Entities;
using ServerTests;
using GameDataTest;
using System.Linq;
using Game.World;

namespace GameUnitTests
{
    [TestFixture]
    public class TechTreeTests
    {
        private TestGame _game;
        private TestServerPlayer _player;
        private PlayerLogic _logic;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _logic = _player.EntityLogic;
        }

        [Test]
        public void RootBuilding_IsAlwaysAvailable()
        {
            // Act
            var result = _logic.CheckTechTree(TestBuildings.CAMP);

            // Assert
            Assert.AreEqual(BuildingTechStatus.Available, result.Status);
            Assert.IsNull(result.BlockedBy);
        }

        [Test]
        public void SecondTierBuilding_RequiresFirstTier()
        {
            var node = _game.Specs.ConstructionTechTree.Root.FindElement(e =>
            {
                return e.Id == TestBuildings.FARM;
            });
            var requirement = node.Parent.Data.Id;

            // Act - Try to check Farm without having Camp
            var result = _logic.CheckTechTree(TestBuildings.FARM);

            // Assert
            Assert.AreEqual(BuildingTechStatus.NotResearched, result.Status);
            Assert.AreEqual(TestBuildings.LUMBER_CAMP, result.BlockedBy);
            Assert.AreEqual((byte)TestBuildings.LUMBER_CAMP, requirement);
        }

        [Test]
        public void SecondTierBuilding_IsAvailableWhenFirstTierBuilt()
        {
            // Arrange - Build Camp
            var tile = _player.GetParty(0).Logic.Map.GetTile();
            tile.Logic.Building.ForceBuild(TestBuildings.CAMP, _player.EntityId);
            tile.GetNeighbor(Direction.NORTH).Logic.Building.ForceBuild(TestBuildings.LUMBER_CAMP, _player.EntityId);

            // Act
            var result = _logic.CheckTechTree(TestBuildings.FARM);

            // Assert
            Assert.AreEqual(BuildingTechStatus.Available, result.Status);
            Assert.IsNull(result.BlockedBy);
        }

        [Test]
        public void ThirdTierBuilding_RequiresSecondTier()
        {
            // Arrange - Build only Camp
            var tile = _player.GetParty(0).Logic.Map.GetTile();
            tile.Logic.Building.ForceBuild(TestBuildings.CAMP, _player.EntityId);

            // Act - Try to check Castle without having Farm
            var result = _logic.CheckTechTree(TestBuildings.FORT);

            // Assert
            Assert.AreEqual(BuildingTechStatus.NotResearched, result.Status);
            Assert.AreEqual(TestBuildings.FARM, result.BlockedBy);
        }

        [Test]
        public void ThirdTierBuilding_IsAvailableWhenAllPrerequisitesBuilt()
        {
            // Arrange - Build Camp and Farm
            var party = _player.GetParty(0);
            var tile1 = party.Logic.Map.GetTile();
            tile1.Logic.Building.ForceBuild(TestBuildings.CAMP, _player.EntityId);

            // Find another tile for the Farm
            var tile2 = _game.FindTile(t =>
                t.Position.X > tile1.Position.X &&
                t.Position.Y == tile1.Position.Y &&
                t.Logic.Tile.GetBuildingOnTile() == null);

            if (tile2 != null)
            {
                tile2.Logic.Building.ForceBuild(TestBuildings.FARM, _player.EntityId);
            }
            else
            {
                Assert.Fail("Could not find a suitable tile for Farm building");
            }

            // Act
            var result = _logic.CheckTechTree(TestBuildings.FORT);

            // Assert
            Assert.AreEqual(BuildingTechStatus.Available, result.Status);
            Assert.IsNull(result.BlockedBy);
        }

        [Test]
        public void BuildingNotInTechTree_IsNotAvailable()
        {
            // Create a building ID that doesn't exist in the tech tree
            var unknownBuilding = new BuildingSpecId(99);

            // Act
            var result = _logic.CheckTechTree(unknownBuilding);

            // Assert
            Assert.AreEqual(BuildingTechStatus.NotInTechTree, result.Status);
        }

        [Test]
        public void TestNotAvailableNotReady()
        {
            var tile = _player.GetParty(0).Logic.Map.GetTile();

            // Act
            var result = _logic.CheckTechTree(TestBuildings.TAVERN);

            // Assert
            Assert.AreEqual(BuildingTechStatus.NotResearched, result.Status);
            Assert.AreEqual((byte)TestBuildings.LUMBER_CAMP, (byte)result.BlockedBy);
        }

        [Test]
        public void BuildingsUnderConstruction_DoNotSatisfyPrerequisites()
        {
            // Arrange
            // Build Camp but leave it under construction
            var tile = _player.GetParty(0).Logic.Map.GetTile();
            var campBuilding = tile.Logic.Building.PlaceConstruction(TestBuildings.LUMBER_CAMP, _player.EntityId);

            // Verify it's under construction
            Assert.IsTrue(campBuilding.Components.Has<ConstructionSiteComponent>());

            // ActCA
            var result = _logic.CheckTechTree(TestBuildings.TAVERN);

            // Assert
            Assert.AreEqual(BuildingTechStatus.NotResearched, result.Status);
            Assert.AreEqual((byte)TestBuildings.LUMBER_CAMP, (byte)result.BlockedBy);
        }


        [Test]
        public void GetBuildingOptions_ReturnsOnlyAvailableBuildings()
        {
            // Arrange - Build Camp
            var tile = _player.GetParty(0).Logic.Map.GetTile();
            tile.Logic.Building.ForceBuild(TestBuildings.CAMP, _player.EntityId);

            // Act
            var buildingOptions = _logic.GetBuildingOptions();

            // Assert
            Assert.IsNotNull(buildingOptions);
            Assert.AreEqual(2, buildingOptions.Count);
            Assert.IsTrue(buildingOptions.Any(b => b.SpecId.Equals(TestBuildings.LUMBER_CAMP)));
            Assert.IsTrue(buildingOptions.Any(b => b.SpecId.Equals(TestBuildings.CAMP)));
            Assert.IsFalse(buildingOptions.Any(b => b.SpecId.Equals(TestBuildings.FARM)));
            Assert.IsFalse(buildingOptions.Any(b => b.SpecId.Equals(TestBuildings.FORT)));
        }

        [Test]
        public void GetBuildingOptions_IncludesAllAvailableOptions()
        {
            // Arrange - Build Camp and Farm
            var party = _player.GetParty(0);
            var tile1 = party.Logic.Map.GetTile();
            tile1.Logic.Building.ForceBuild(TestBuildings.CAMP, _player.EntityId);

            // Find another tile for the Farm
            var tile2 = _game.FindTile(t =>
                t.Position.X > tile1.Position.X &&
                t.Position.Y == tile1.Position.Y &&
                t.EntityType != EntityType.Building);

            if (tile2 != null)
            {
                tile2.Logic.Building.ForceBuild(TestBuildings.LUMBER_CAMP, _player.EntityId);
            }
            else
            {
                Assert.Fail("Could not find a suitable tile for Farm building");
            }

            // Act
            var buildingOptions = _logic.GetBuildingOptions();

            // Assert
            Assert.IsNotNull(buildingOptions);

            // Should include both lower-tier buildings and newly unlocked buildings
            Assert.IsTrue(buildingOptions.Any(b => b.SpecId.Equals(TestBuildings.CAMP)));
            Assert.IsTrue(buildingOptions.Any(b => b.SpecId.Equals(TestBuildings.FARM)));
            Assert.IsTrue(buildingOptions.Any(b => b.SpecId.Equals(TestBuildings.TAVERN)));
        }

        [Test]
        public void BuildingDependenciesFormCorrectPath()
        {
            // Arrange - We need to build Camp and Farm
            var party = _player.GetParty(0);
            var tile1 = party.Logic.Map.GetTile();
            tile1.Logic.Building.ForceBuild(TestBuildings.CAMP, _player.EntityId);

            // Act - Test the dependency path for Castle
            // First check without Farm - should require Farm
            var initialResult = _logic.CheckTechTree(TestBuildings.FORT);

            // Then build Farm
            var tile2 = _game.FindTile(t =>
                t.Position.X > tile1.Position.X &&
                t.Position.Y == tile1.Position.Y &&
                t.EntityType != EntityType.Building);

            if (tile2 == null)
            {
                Assert.Fail("Could not find a suitable tile for Farm building");
            }

            tile2.Logic.Building.ForceBuild(TestBuildings.FARM, _player.EntityId);

            // Now check again, should be available
            var finalResult = _logic.CheckTechTree(TestBuildings.FORT);

            // Assert
            Assert.AreEqual(BuildingTechStatus.NotResearched, initialResult.Status);
            Assert.AreEqual(TestBuildings.FARM, initialResult.BlockedBy);

            Assert.AreEqual(BuildingTechStatus.Available, finalResult.Status);
            Assert.IsNull(finalResult.BlockedBy);
        }

        /*
        [Test]
        public void DestroyingPrerequisiteBuilding_BlocksHigherTierBuildings()
        {
            // Arrange - Build Camp and Farm
            var party = _player.GetParty(0);
            var tile1 = party.Logic.Map.GetTile();
            var campBuilding = tile1.Logic.Building.ForceBuild(TestBuildings.CAMP, _player.EntityId);

            // Build Farm
            var tile2 = _game.FindTile(t =>
                t.Position.X > tile1.Position.X &&
                t.Position.Y == tile1.Position.Y &&
                t.EntityType != EntityType.Building);

            if (tile2 == null)
            {
                Assert.Fail("Could not find a suitable tile for Farm building");
            }

            var farmBuilding = tile2.Logic.Building.ForceBuild(TestBuildings.FARM, _player.EntityId);

            // Verify Castle is available
            var initialResult = _logic.CheckTechTree(TestBuildings.CASTLE);
            Assert.AreEqual(BuildingTechStatus.Available, initialResult.Status);

            // Act - Destroy the Farm (remove it from the game)
            _game.Entities.DestroyEntity(farmBuilding.EntityId);

            // Assert
            var finalResult = _logic.CheckTechTree(TestBuildings.CASTLE);
            Assert.AreEqual(BuildingTechStatus.NotResearched, finalResult.Status);
            Assert.AreEqual(TestBuildings.FARM, finalResult.BlockedBy);
        }
        */
    }
}
