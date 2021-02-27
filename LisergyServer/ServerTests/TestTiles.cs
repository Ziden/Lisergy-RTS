using Game;
using Game.World;
using GameDataTest;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace Tests
{
    public class TestTile
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame();
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
        public void TestTilePassableFlag()
        {
            Assert.That(TestTiles.MOUNTAIN.MovementFactor == 0, "Mountain has to be totally impassable");
            Assert.That(TestTiles.GRASS.MovementFactor != 0, "Grass has to be passable");

            var tile = Game.World.GetTile(0, 0);

            tile.TileId = TestTiles.GRASS.ID;
            Assert.IsTrue(tile.Passable);
            Assert.AreEqual(tile.MovementFactor, TestTiles.GRASS.MovementFactor);

            tile.TileId = TestTiles.MOUNTAIN.ID;
            Assert.IsFalse(tile.Passable);
            Assert.AreEqual(tile.MovementFactor, TestTiles.MOUNTAIN.MovementFactor);
        }
    }
}