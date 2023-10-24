using Game.World;
using NUnit.Framework;

namespace UnitTests
{
    public class TestWorld
    {
        [Test]
        public void TestSimpleCreation()
        {
            var world = new GameWorld(16, 16);

            var tile = world.Map.GetTile(1, 2);

            Assert.AreEqual(1, tile.X);
            Assert.AreEqual(2, tile.Y);
            Assert.AreEqual(0, tile.SpecId);
        }        
    }

}
