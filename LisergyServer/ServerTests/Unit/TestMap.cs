using Game.World;
using NUnit.Framework;
using ServerTests;

namespace UnitTests
{
    public class TestMap
    {
        [Test]
        public void TestMapCreation()
        {
            var game = new TestGame(createWorld: false);
            var world = new GameWorld(game, 8, 8);
            game.SetupWorld(world);
            var player = game.CreatePlayer(4, 4);

            Assert.AreEqual(player, world.Players[player.EntityId]);
        }
    }
}