using Game.Entities;
using Game.World;
using GameDataTest;
using NUnit.Framework;
using ServerTests;
using System.Linq;
using Tests.Unit.Stubs;

namespace UnitTests
{
    public unsafe class TestTile
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame();
        }

        [Test]
        public void TestTileAllocation()
        {

        }

        [Test]
        public void TestNeighboars()
        {
            var tile = Game.World.GetTile(1, 1);

            Assert.AreEqual(Game.World.GetTile(1, 1 + 1), tile.GetNeighbor(Direction.NORTH));
            Assert.AreEqual(Game.World.GetTile(1, 1 - 1), tile.GetNeighbor(Direction.SOUTH));
            Assert.AreEqual(Game.World.GetTile(1 - 1, 1), tile.GetNeighbor(Direction.WEST));
            Assert.AreEqual(Game.World.GetTile(1 + 1, 1), tile.GetNeighbor(Direction.EAST));
        }

        [Test]
        public void TestDirections()
        {
            var tile = Game.World.GetTile(1, 1);

            Assert.AreEqual(tile.GetDirection(Game.World.GetTile(1, 1 + 1)), Direction.NORTH);
            Assert.AreEqual(tile.GetDirection(Game.World.GetTile(1, 1 - 1)), Direction.SOUTH);
            Assert.AreEqual(tile.GetDirection(Game.World.GetTile(1 + 1, 1)), Direction.EAST);
            Assert.AreEqual(tile.GetDirection(Game.World.GetTile(1 - 1, 1)), Direction.WEST);
        }

        [Test]
        public void TestAddingStaticEntity()
        {
            var player = Game.GetTestPlayer();
            var building = player.Buildings.FirstOrDefault();
            var tile = building.GetTile().GetNeighbor(Direction.NORTH);

            var dungeon = Game.Entities.CreateEntity(EntityType.Dungeon);
            dungeon.Logic.Map.SetPosition(tile);

            Assert.AreEqual(dungeon.GetTile(), tile);
        }

        [Test]
        public void TestTilePassableFlag()
        {
            Assert.That(TestTiles.MOUNTAIN.MovementFactor == 0, "Mountain has to be totally impassable");
            Assert.That(TestTiles.GRASS.MovementFactor != 0, "Grass has to be passable");

            var tile = Game.World.GetTile(0, 0);

            tile.Logic.Tile.SetTileId(TestTiles.GRASS.ID);
            Assert.IsTrue(tile.Logic.Tile.IsPassable());
            Assert.AreEqual(tile.Logic.Tile.GetMovementFactor(), TestTiles.GRASS.MovementFactor);

            tile.Logic.Tile.SetTileId(TestTiles.MOUNTAIN.ID);
            Assert.IsFalse(tile.Logic.Tile.IsPassable());
            Assert.AreEqual(tile.Logic.Tile.GetMovementFactor(), TestTiles.MOUNTAIN.MovementFactor);
        }

        [Test]
        public void TestTileId()
        {
            var tile = Game.World.GetTile(5, 5);
            var tile2 = Game.World.GetTile(6, 6);

            Assert.AreNotEqual(tile.EntityId, tile2.EntityId);
        }

        [Test]
        public void TestTileIdSame()
        {
            var tile = Game.World.GetTile(5, 5);
            var tile2 = Game.World.GetTile(5, 5);

            Assert.AreEqual(tile.EntityId, tile2.EntityId);
        }

        [Test]
        public void TestTileUniqueIds()
        {
            var tile = Game.World.GetTile(5, 5);
            var tile2 = Game.World.GetTile(5, 5);

            Assert.AreEqual(tile.EntityId, tile2.EntityId);
        }
    }
}