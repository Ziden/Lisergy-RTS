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
            var serialized = Serialization.FromPacketRaw(new GameSpecPacket(game));
            var deserialized = (GameSpecPacket)Serialization.ToPacketRaw(serialized);

            Assert.AreEqual(game.Specs.Units.Count, deserialized.Spec.Units.Count);
        }
    }
}