using Game;
using Game.Events.ServerEvents;
using GameDataTest;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace Tests
{
    public class TestPathfinding
    {
        private TestGame Game;

        [SetUp]
        public void SetupTests()
        {
            GameWorld world = new GameWorld(10, 50, 50);
          
            Game = new TestGame(world);
        }

        [Test]
        public void TestSimplePath()
        {
            Game.World.GetTile(1, 0).TileId = TestTiles.MOUNTAIN.ID;
            Game.World.GetTile(1, 1).TileId = TestTiles.MOUNTAIN.ID;
            Game.World.GetTile(1, 2).TileId = TestTiles.MOUNTAIN.ID;

            var asd = Game.World.GetTile(2, 3);
            var asd2 = Game.World.GetTile(3, 3);
            var asd3 = Game.World.GetTile(3, 3);

            var from = Game.World.GetTile(0, 0);
            var to = Game.World.GetTile(2, 0);
            var path = Game.World.Map.FindPath(from, to);

            Assert.NotNull(path);
            Assert.That(path.Count == 9);
            Assert.That(path[0].X == 0 && path[0].Y == 0);
            Assert.That(path[1].X == 0 && path[1].Y == 1);
            Assert.That(path[2].X == 0 && path[2].Y == 2);
            Assert.That(path[3].X == 0 && path[3].Y == 3);
            Assert.That(path[4].X == 1 && path[4].Y == 3);
            Assert.That(path[5].X == 2 && path[5].Y == 3);
            Assert.That(path[6].X == 2 && path[6].Y == 2);
            Assert.That(path[7].X == 2 && path[7].Y == 1);
            Assert.That(path[8].X == 2 && path[8].Y == 0);
        }
    }
}