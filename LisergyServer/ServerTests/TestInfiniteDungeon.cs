using Game.Events.ClientEvents;
using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace Tests
{
    public class TestInfiniteDungeon
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame();
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void TestJoin()
        {
            var pl = Game.GetTestPlayer();
        
        }
    }
}
