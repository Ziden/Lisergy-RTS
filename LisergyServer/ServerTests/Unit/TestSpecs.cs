using Game.Engine;
using Game.Events.ServerEvents;
using GameData;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace GameUnitTests
{
    public class TestSpecSerialization
    {
        [Test]
        public void TestTechTreeSerialization()
        {
            var game = new TestGame();
            var packet = new GameSpecPacket(game);
            var serialized = Serialization.FromAnyType(packet);
            var deserialized = Serialization.ToAnyType<GameSpecPacket>(serialized);
            deserialized.OnAfterDeserialize();

            var oldTreeFlat = game.Specs.ConstructionTechTree.Root.Flatten();
            var newTreeFlat = deserialized.Spec.ConstructionTechTree.Root.Flatten();

            Assert.IsTrue(oldTreeFlat.SequenceEqual(newTreeFlat));
            foreach (var newItem in newTreeFlat)
            {
                var oldNode = game.Specs.ConstructionTechTree.Root.FindElement(e =>
                {
                    return e.Id == newItem;
                });
                var newNode = deserialized.Spec.ConstructionTechTree.Root.FindElement(e =>
                {
                    return e.Id == newItem;
                });
                Assert.AreEqual(oldNode.Parent?.Data, newNode.Parent?.Data);
                Assert.IsTrue(oldNode.Children().SequenceEqual(newNode.Children()));
            }
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