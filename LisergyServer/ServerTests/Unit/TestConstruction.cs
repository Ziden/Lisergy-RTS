using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Scheduler;
using Game.Entities;
using Game.Events.ServerEvents;
using Game.Systems.Building;
using Game.Systems.Course;
using Game.Systems.Movement;
using Game.Systems.Resources;
using Game.Tile;
using Game.World;
using GameDataTest;
using NUnit.Framework;
using ServerTests;
using System.Collections.Generic;
using System.Linq;
using Tests.Unit.Stubs;

namespace GameUnitTests
{
    public class TestConstruction
    {
        private TestGame _game;
        private TestServerPlayer _player;
        private IEntity _party;
        private GameScheduler _scheduler;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _party = _player.GetParty(0);
            _scheduler = _game.Scheduler as GameScheduler;
        }

        [Test]
        public void TestBuildingOnResource()
        {
            var blockedTile = _party.GetTile().GetNeighbor(Direction.NORTH);
            blockedTile.Logic.Tile.SetTileId(TestTiles.FOREST.ID);

            Assert.AreEqual(BuildResult.HasResource, _party.Logic.Building.CanBuildOnTile(TestBuildings.CAMP, blockedTile));
        }

        [Test]
        public void TestBuildingBlocked()
        {
            var blockedTile = _party.GetTile().GetNeighbor(Direction.NORTH);
            blockedTile.Logic.Tile.SetTileId(TestTiles.MOUNTAIN.ID);

            Assert.AreEqual(BuildResult.Blocked, _party.Logic.Building.CanBuildOnTile(TestBuildings.CAMP, blockedTile));
        }

        [Test]
        public void TestBuilding()
        {
            var blockedTile = _party.GetTile().GetNeighbor(Direction.NORTH);
            blockedTile.Logic.Building.ForceBuild(TestBuildings.CAMP, GameId.ZERO);

            Assert.AreEqual(BuildResult.HasBuilding, _party.Logic.Building.CanBuildOnTile(TestBuildings.CAMP, blockedTile));
        }

        [Test]
        public void TestWontStartBuildingIfBlocked()
        {
            var blockedTile = _party.GetTile().GetNeighbor(Direction.NORTH);
            blockedTile.Logic.Building.ForceBuild(TestBuildings.CAMP, GameId.ZERO);

            var construction = blockedTile.Logic.Building.StartConstruction(TestBuildings.CAMP, _player.EntityId);

            Assert.IsNull(construction);
        }

        [Test]
        public void TestStartBuilding()
        {
            var blockedTile = _party.GetTile().GetNeighbor(Direction.NORTH);
            
            var construction = blockedTile.Logic.Building.StartConstruction(TestBuildings.CAMP, _player.EntityId);

            var spec = _game.Specs.BuildingConstructions[TestBuildings.CAMP];

            var component = construction.Get<ConstructionComponent>();

            Assert.NotNull(component);
            Assert.IsNull(component.TimeBlock); 
        }

        [Test]
        public void TestAssignBuilder()
        {
            var blockedTile = _party.GetTile().GetNeighbor(Direction.NORTH);

            var construction = blockedTile.Logic.Building.StartConstruction(TestBuildings.CAMP, _player.EntityId);

            var spec = _game.Specs.BuildingConstructions[TestBuildings.CAMP];

            var component = construction.Get<ConstructionComponent>();

            Assert.NotNull(component);
            Assert.IsNull(component.TimeBlock);
        }
    }
}