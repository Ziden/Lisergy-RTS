using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Scheduler;
using Game.Systems.Building;
using Game.Tile;
using Game.World;
using GameDataTest;
using NUnit.Framework;
using ServerTests;
using Tests.Unit.Stubs;

namespace GameUnitTests
{
    public class TestConstruction
    {
        private TestGame _game;
        private TestServerPlayer _player;
        private IEntity _party;
        private TileModel _tile;
        private GameScheduler _scheduler;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _party = _player.GetParty(0);
            _tile = _party.GetTile();
            _scheduler = _game.Scheduler as GameScheduler;
        }

        [Test]
        public void TestBuildingOnResource()
        {
            var blockedTile = _party.GetTile().GetNeighbor(Direction.NORTH);
            blockedTile.Logic.Tile.SetTileId(TestTiles.FOREST.ID);

            Assert.AreEqual(BuildResult.HasResource, blockedTile.Logic.Building.IsTileFreeForBuilding(TestBuildings.CAMP));
        }

        [Test]
        public void TestBuildingBlocked()
        {
            var blockedTile = _party.GetTile().GetNeighbor(Direction.NORTH);
            blockedTile.Logic.Tile.SetTileId(TestTiles.MOUNTAIN.ID);

            Assert.AreEqual(BuildResult.Blocked, blockedTile.Logic.Building.IsTileFreeForBuilding(TestBuildings.CAMP));
        }

        [Test]
        public void TestBuilding()
        {
            var blockedTile = _party.GetTile().GetNeighbor(Direction.NORTH);
            blockedTile.Logic.Building.ForceBuild(TestBuildings.CAMP, GameId.ZERO);

            Assert.AreEqual(BuildResult.HasBuilding, blockedTile.Logic.Building.IsTileFreeForBuilding(TestBuildings.CAMP));
        }

        [Test]
        public void TestWontStartBuildingIfBlocked()
        {
            var blockedTile = _party.GetTile().GetNeighbor(Direction.NORTH);
            blockedTile.Logic.Building.ForceBuild(TestBuildings.CAMP, GameId.ZERO);

            var construction = blockedTile.Logic.Building.PlaceConstruction(TestBuildings.CAMP, _player.EntityId);

            Assert.IsNull(construction);
        }

        [Test]
        public void TestStartBuilding()
        {
            var blockedTile = _party.GetTile().GetNeighbor(Direction.NORTH);

            var construction = blockedTile.Logic.Building.PlaceConstruction(TestBuildings.CAMP, _player.EntityId);

            var spec = _game.Specs.BuildingConstructions[TestBuildings.CAMP];

            var component = construction.Get<ConstructionSiteComponent>();

            Assert.NotNull(construction);
            Assert.NotNull(component);
            Assert.IsNull(component.BuildingWorkPrediction);
        }

        [Test]
        public void TestAssignBuilder()
        {
            var tile = _party.GetTile().GetNeighbor(Direction.NORTH);
            var construction = tile.Logic.Building.PlaceConstruction(TestBuildings.CAMP, _player.EntityId);

            Assert.IsTrue(construction.Logic.Building.AddBuilder(_party));

            var component = construction.Get<ConstructionSiteComponent>();
            var builderComponent = _party.Get<ConstructionWorkerComponent>();

            Assert.NotNull(component);
            Assert.NotNull(component.BuildingWorkPrediction);
            Assert.AreEqual(0, component.Percentage);
            Assert.AreEqual(tile.Position, builderComponent.BuildingAt);
        }

        [Test]
        public void TestBuildHalfTime()
        {
            var tile = _party.GetTile().GetNeighbor(Direction.NORTH);
            var construction = tile.Logic.Building.PlaceConstruction(TestBuildings.CAMP, _player.EntityId);

            Assert.IsTrue(construction.Logic.Building.AddBuilder(_party));

            var component = construction.Get<ConstructionSiteComponent>();

            _scheduler.SetLogicalTime(_scheduler.LogicalTime.Add(component.BuildingWorkPrediction.TotalBlockTime / 2));

            _party.Logic.Map.SetPosition(tile); // moving the party should stop building

            component = construction.Get<ConstructionSiteComponent>();

            Assert.False(_party.Components.Has<ConstructionWorkerComponent>());
            Assert.Null(component.BuildingWorkPrediction); // not building anymore
            Assert.AreEqual(50, component.Percentage);
        }

        [Test]
        public void TestBuildComplete()
        {
            var tile = _party.GetTile().GetNeighbor(Direction.NORTH);
            var construction = tile.Logic.Building.PlaceConstruction(TestBuildings.CAMP, _player.EntityId);

            Assert.IsTrue(construction.Logic.Building.AddBuilder(_party));

            var component = construction.Get<ConstructionSiteComponent>();

            _scheduler.SetLogicalTime(_scheduler.LogicalTime.Add(component.BuildingWorkPrediction.TotalBlockTime));

            _party.Logic.Map.SetPosition(tile); // moving the party should stop building

            component = construction.Get<ConstructionSiteComponent>();
            
            Assert.False(_party.Components.Has<ConstructionWorkerComponent>()); 
            Assert.Null(component);
        }
    }
}