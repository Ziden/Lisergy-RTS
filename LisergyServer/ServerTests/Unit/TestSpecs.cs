using Game;
using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests;

namespace Tests
{
    public class TestSpecSerialization
    { 
        [Test]
        public void TestBasicSerialization()
        {
            var game = new TestGame();
            var serialized = Serialization.FromEventRaw(new GameSpecPacket(game));
            var deserialized = (GameSpecPacket)Serialization.ToEventRaw(serialized);

            Assert.AreEqual(game.Specs.Units.Count, deserialized.Spec.Units.Count);
        }
    }
}