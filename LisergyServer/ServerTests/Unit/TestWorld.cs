using Game.World;
using NUnit.Framework;
using ServerTests;

namespace GameUnitTests
{
    public class TestGameWorld
    {
        [Test]
        public void TestSimpleCreation()
        {
            var game = new TestGame();
            var world = new GameWorld(game, 16, 16);

            var tile = world.GetTile(1, 2);

            Assert.AreEqual(1, tile.X);
            Assert.AreEqual(2, tile.Y);
            Assert.AreEqual(0, tile.SpecId);
        }
    }

}
