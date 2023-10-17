using Game.Events.ServerEvents;
using Game.Systems.Building;
using Game.Systems.MapPosition;
using Game.Systems.Tile;
using NUnit.Framework;
using ServerTests;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
        public void TestInitialBuilding()
        {
            var player = Game.GetTestPlayer();

            var initialBuildingSpec = Game.Specs.InitialBuilding;
            var building = player.Data.Buildings.FirstOrDefault();
            var tile = building.Components.GetReference<MapReferenceComponent>().Tile;
            Assert.IsTrue(player.Data.Buildings.Count == 1);
            Assert.IsTrue(player.Data.Buildings.Any(b => b.SpecId == initialBuildingSpec.Id));
            Assert.IsTrue(tile.Components.GetReference<TileHabitants>().Building == player.Data.Buildings.First());
            Assert.IsTrue(((PlayerBuildingEntity)tile.Components.GetReference<TileHabitants>().Building).SpecId == initialBuildingSpec.Id);
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

            player.EntityLogic.Player.Build(buildingSpec.Id, tile);

            Assert.IsTrue(player.Data.Buildings.Count == 2);
            Assert.IsTrue(player.Data.Buildings.Any(b => b.SpecId == buildingSpec.Id));
            Assert.IsTrue(tile.Components.GetReference<TileHabitants>().Building == player.Data.Buildings.Last());
            Assert.IsTrue(((PlayerBuildingEntity)tile.Building).SpecId == buildingSpec.Id);
            Assert.That(tile.EntitiesViewing.Contains(tile.Building));

        }

        [Test]
        public void TestPlacingBuildingSendingUpdateEvents()
        {
            var player = Game.GetTestPlayer();
            var initialBuildingSpec = Game.RandomBuildingSpec();
            var tile = Game.RandomNotBuiltTile();
            var buildingSpec = Game.RandomBuildingSpec();
            Game.Entities.DeltaCompression.ClearDeltas();
            Game.SentServerPackets.Clear();

            var building = player.EntityLogic.Player.Build(buildingSpec.Id, tile);
            Game.Entities.DeltaCompression.SendDeltaPackets(player);
            var buildingPacket = Game.SentServerPackets.First(o => o is EntityUpdatePacket p && p.EntityId == building.EntityId);

            Assert.NotNull(buildingPacket);

        }
    }
}