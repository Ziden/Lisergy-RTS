using Game;
using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests;

namespace Tests
{
    public class TestSpecSerialization
    {

        [SetUp]
        public void Setup()
        {

        }

        [TearDown]
        public void Tear()
        {

        }

        [Test]
        public void TestBasicSerialization()
        {
            var game = new TestGame();
            var serialized = Serialization.FromEventRaw(new GameSpecPacket(game));
            var deserialized = (GameSpecPacket)Serialization.ToEventRaw(serialized);

            Assert.AreEqual(GameLogic.Specs.Units.Values, deserialized.Spec.Units.Values);
        }
    }
}