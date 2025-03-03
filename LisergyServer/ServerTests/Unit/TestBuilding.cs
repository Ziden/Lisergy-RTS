using Game.Events.ServerEvents;
using Game.Systems.Building;
using Game.Systems.Tile;
using NUnit.Framework;
using ServerTests;
using System.Linq;
using Tests.Unit.Stubs;

namespace UnitTests
{
    public class TestBuilding
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame();
        }

        [Test]

        public void TestRemovedBuildingTileReference()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();

            var initialBuildingSpec = Game.Specs.InitialBuilding;
            var building = player.Buildings.FirstOrDefault();

            Assert.NotNull(building.GetTile());

            building.Logic.Map.SetPosition(null);

            Assert.IsNull(building.GetTile());
        }

        [Test]
        public void TestInitialBuilding()
        {
            var player = Game.GetTestPlayer();

            var initialBuildingSpec = Game.Specs.InitialBuilding;
            var building = player.Buildings.FirstOrDefault();
            var tile = building.GetTile();
            var buildingThere = tile.Components.Get<TileHabitantsComponent>().Building;
            Assert.IsTrue(player.Buildings.Count == 1);
            Assert.IsTrue(player.Buildings.Any(b => b.Get<PlayerBuildingComponent>().SpecId == initialBuildingSpec.SpecId));
            Assert.IsTrue(buildingThere == player.Buildings.First());
            Assert.IsTrue(buildingThere.Get<PlayerBuildingComponent>().SpecId == initialBuildingSpec.SpecId);
        }

        [Test]
        public void TestUnbuiltTile()
        {
            var tile = Game.RandomNotBuiltTile();
        }

        [Test]
        public void TestNewBuilding()
        {
            var player = Game.GetTestPlayer();
            var initialBuildingSpec = Game.RandomBuildingSpec();
            var tile = Game.RandomNotBuiltTile();
            var buildingSpec = Game.RandomBuildingSpec();

            player.EntityLogic.Build(buildingSpec.SpecId, tile);

            Assert.IsTrue(player.Buildings.Count == 2);
            Assert.IsTrue(player.Buildings.Any(b => b.Get<PlayerBuildingComponent>().SpecId == initialBuildingSpec.SpecId));
            Assert.IsTrue(tile.Components.Get<TileHabitantsComponent>().Building == player.Buildings.Last());
            Assert.IsTrue(tile.Logic.Tile.GetBuildingOnTile().Get<PlayerBuildingComponent>().SpecId == buildingSpec.SpecId);
            Assert.That(tile.Logic.Vision.GetEntitiesViewing().Contains(tile.Logic.Tile.GetBuildingOnTile().EntityId));

        }

        [Test]
        public void TestPlacingBuildingSendingUpdateEvents()
        {
            var player = Game.GetTestPlayer();
            var initialBuildingSpec = Game.RandomBuildingSpec();
            var tile = Game.RandomNotBuiltTile();
            var buildingSpec = Game.RandomBuildingSpec();
            Game.Network.DeltaCompression.ClearDeltas();
            Game.SentServerPackets.Clear();

            var building = player.EntityLogic.Build(buildingSpec.SpecId, tile);
            Game.Network.DeltaCompression.SendAllModifiedEntities(player.EntityId);
            var buildingPacket = Game.SentServerPackets.First(o => o is EntityUpdatePacket p && p.EntityId == building.EntityId);

            Assert.NotNull(buildingPacket);

        }
    }
}