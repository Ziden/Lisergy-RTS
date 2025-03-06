using Game.Engine;
using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests;

namespace GameUnitTests
{
    public class TestSpecSerialization
    {
        [Test]
        public void TestBasicSerialization()
        {
            var game = new TestGame();
            var serialized = Serialization.FromAnyType((object)new GameSpecPacket(game));
            var deserialized = Serialization.ToAnyType<GameSpecPacket>(serialized);

            Assert.AreEqual(game.Specs.Units.Count, deserialized.Spec.Units.Count);
        }
    }
}