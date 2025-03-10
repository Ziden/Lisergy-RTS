using Game.Engine;
using Game.Events.ServerEvents;
using GameData;
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
            var serialized = Serialization.FromAnyType(new GameSpecPacket(game));
            var deserialized = Serialization.ToAnyType<GameSpecPacket>(serialized);

            Assert.AreEqual(game.Specs.Units.Count, deserialized.Spec.Units.Count);
        }

        [Test]
        public void TestSpecIdSerialization()
        {
            var game = new TestGame();
            var serialized = Serialization.FromAnyType(new BuildingSpecId() { Id = 2 });
            var deserialized = Serialization.ToAnyType<BuildingSpecId>(serialized);

            Assert.AreEqual((byte)2, (byte)deserialized);
        }
    }
}