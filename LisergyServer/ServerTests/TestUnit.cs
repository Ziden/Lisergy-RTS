using Game;
using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace Tests
{
    public class TestUnits
    {
        private TestGame Game;

        [SetUp]
        public void SetupTests()
        {
            Game = new TestGame();
        }

        [Test]
        public void TestInitialUnit()
        {
            var player = Game.GetTestPlayer();
            var unit = player.Units.First();

            Assert.AreEqual(1, player.Units.Count);
            Assert.AreEqual(TestGame.Specs.InitialUnit, unit.SpecID);
            Assert.That(unit.Party.Tile.Parties.Contains(unit.Party));
        }

        [Test]
        public void TestUnitVisibleEvent()
        {
            var player = Game.GetTestPlayer();
            var unit = player.Units.First();
            var building = player.Buildings.First();
            var tile = unit.Party.Tile;

            var visibleEvent = Game.ReceivedEvents.Where(e => e is PartyVisibleEvent).FirstOrDefault() as PartyVisibleEvent;

            Assert.That(visibleEvent != null);
            Assert.AreEqual(unit.Party.Id, visibleEvent.Party.Id);
            Assert.AreEqual(building, visibleEvent.Viewer);
        }
    }
}