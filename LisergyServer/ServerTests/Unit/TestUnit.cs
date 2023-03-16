using Game;
using Game.Events.ServerEvents;
using Game.World.Components;
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

            var bt = player.Buildings.First().Tile;

            var building = Game.World.GetTile(10, 10).GetComponent<EntityPlacementComponent>();

            Assert.AreEqual(1, player.Units.Count);
            Assert.AreEqual(TestGame.Specs.InitialUnit, unit.SpecId);
            Assert.That(unit.Party.Tile.GetComponent<EntityPlacementComponent>().EntitiesIn.Contains(unit.Party));
        }


        [Test]
        public void TestUnitTileReference()
        {
            var player = Game.GetTestPlayer();
            var unit = player.Units.First();

            var tile = Game.World.GetTile(unit.Party.Tile.X, unit.Party.Tile.Y);

            Assert.AreEqual(tile, unit.Party.Tile);
        }

        [Test]
        public void TestUnitVisibleEvent()
        {
            var player = Game.GetTestPlayer();
            var unit = player.Units.First();
            var building = player.Buildings.First();
            var tile = unit.Party.Tile;

            var visibleEvent = Game.ReceivedEvents.Where(e => e is EntityUpdatePacket && ((EntityUpdatePacket)e).Entity.Id == unit.Party.Id).FirstOrDefault() as EntityUpdatePacket;

            Assert.That(visibleEvent != null);
            Assert.AreEqual(unit.Party.Id, visibleEvent.Entity.Id);
        }
    }
}