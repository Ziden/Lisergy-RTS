using Game;
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