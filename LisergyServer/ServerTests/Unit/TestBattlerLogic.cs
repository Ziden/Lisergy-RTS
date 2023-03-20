using NUnit.Framework;

namespace ServerTests.Unit
{
    public class TestBattlerLogic
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame();
        }

        [Test]
        public void TestSyncTriggersBattleidChanges()
        {

        }

    }
}
