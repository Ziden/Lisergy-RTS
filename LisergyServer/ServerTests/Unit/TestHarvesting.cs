using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Scheduler;
using Game.Entities;
using Game.Events.ServerEvents;
using Game.Systems.Course;
using Game.Systems.Movement;
using Game.Systems.Resources;
using Game.Tile;
using Game.World;
using GameDataTest;
using NUnit.Framework;
using ServerTests;
using System.Collections.Generic;
using System.Linq;
using Tests.Unit.Stubs;

namespace GameUnitTests
{
    public class TestHarvesting
    {
        private TestGame _game;
        private TestServerPlayer _player;
        private IEntity _party;
        private TileModel _logs;
        private GameScheduler _scheduler;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _party = _player.GetParty(0);
            _logs = _game.WorldChunks.GetTile(5, 5);
            _party.Logic.Map.SetPosition(_logs.GetNeighbor(Direction.SOUTH));
            _scheduler = _game.Scheduler as GameScheduler;
            _logs.Logic.Tile.SetTileId(TestTiles.FOREST.ID);
        }

        [Test]
        public void TestCanHarvest()
        {
            Assert.IsTrue(_party.Logic.Harvesting.CanHarvest(_logs));
        }

        [Test]
        public void TestCannotHarvestIfFar()
        {
            _party.Logic.Map.SetPosition(_party.GetTile().GetNeighbor(Direction.SOUTH));

            Assert.IsFalse(_party.Logic.Harvesting.CanHarvest(_logs));
        }

        [Test]
        public void TestBeginHarvesting()
        {
            var harvesting = _party.Logic.Harvesting.StartHarvesting(_logs);

            Assert.IsTrue(harvesting);

            var harvestingComponent = _party.Get<HarvestingComponent>();
            var tileResource = _logs.Get<TileResourceComponent>();

            Assert.AreEqual(true, tileResource.BeingHarvested);
            Assert.AreEqual(_logs.Position, harvestingComponent.Tile);
            Assert.AreEqual(_game.GameTime.ToBinary(), harvestingComponent.StartedAt);
        }

        [Test]
        public void TestStartHarvestingSendingUpdatePackets()
        {
            _game.Network.DeltaCompression.ClearDeltas();
            _game.SentServerPackets.Clear();

            Assert.IsTrue(_party.Logic.Harvesting.StartHarvesting(_logs));

            _game.Network.DeltaCompression.SendAllModifiedEntities(_party.OwnerID);

            var tileUpdates = _game.SentServerPackets.Where(p => p is EntityUpdatePacket up && up.Type == EntityType.Tile && up.EntityId == _logs.EntityId).ToList();
            Assert.AreEqual(1, tileUpdates.Count);
        }

        [Test]
        public void TestFinishHarvestingAll()
        {
            var tileResourceBefore = _logs.Get<TileResourceComponent>().FastShallowClone();

            _party.Logic.Harvesting.StartHarvesting(_logs);
            var totalTime = _logs.HarvestPointSpec.ResourceAmount * _logs.HarvestPointSpec.HarvestTimePerUnit;

            // Advance all harvesting time
            _scheduler.SetLogicalTime(_game.GameTime + totalTime);

            var harvestedStack = _party.Logic.Harvesting.StopHarvesting();

            var tileResourceAfter = _logs.Get<TileResourceComponent>();
            var resourcedSubtracted = tileResourceBefore.Resource.Amount - tileResourceAfter.Resource.Amount;
            var partyCargo = _party.Get<CargoComponent>();

            Assert.AreEqual(resourcedSubtracted, partyCargo.GetAmount(tileResourceAfter.Resource.ResourceId));
            Assert.AreEqual(resourcedSubtracted, harvestedStack.Amount);
            Assert.IsFalse(_party.Components.Has<HarvestingComponent>());
            Assert.AreEqual(false, tileResourceAfter.BeingHarvested);
        }

        [Test]
        public void TestFinishHarvestingPackets()
        {
            var tileResourceBefore = _logs.Get<TileResourceComponent>();

            _party.Logic.Harvesting.StartHarvesting(_logs);
            var totalTime = _logs.HarvestPointSpec.ResourceAmount * _logs.HarvestPointSpec.HarvestTimePerUnit;
            _scheduler.SetLogicalTime(_game.GameTime + totalTime);

            _game.Network.DeltaCompression.ClearDeltas();
            _player.ReceivedPackets.Clear();

            var harvestedStack = _party.Logic.Harvesting.StopHarvesting();
            _game.Network.DeltaCompression.SendAllModifiedEntities(_player.EntityId);

            var tileUpdate = _player.ReceivedEntityUpdates(EntityType.Tile).First();

            Assert.That(tileUpdate.SyncedComponents.Any(c => c.GetType() == typeof(TileResourceComponent)));

        }

        [Test]
        public unsafe void TestHarvestingComponentSync()
        {
            var tileResourceBefore = _logs.Get<TileResourceComponent>();
            _game.Network.DeltaCompression.ClearDeltas();
            _party.Logic.Harvesting.StartHarvesting(_logs);
            var totalTime = _logs.HarvestPointSpec.ResourceAmount * _logs.HarvestPointSpec.HarvestTimePerUnit;
            var startTime = _game.GameTime;

            var entityUpdate1 = _party.Logic.DeltaCompression.GetUpdatePacket(_player.EntityId) as EntityUpdatePacket;

            // Advance all harvesting time
            _scheduler.SetLogicalTime(_game.GameTime + totalTime);
            _game.Network.DeltaCompression.ClearDeltas();
            var harvestedStack = _party.Logic.Harvesting.StopHarvesting();

            var entityUpdate2 = _party.Logic.DeltaCompression.GetUpdatePacket(_player.EntityId) as EntityUpdatePacket;

            var firstUpdate = entityUpdate1.SyncedComponents.FirstOrDefault(c => c.GetType() == typeof(HarvestingComponent));
            var secondUpdate = entityUpdate2.SyncedComponents.FirstOrDefault(c => c.GetType() == typeof(HarvestingComponent));

            Assert.IsNull(secondUpdate);
            Assert.AreEqual(startTime.ToBinary(), ((HarvestingComponent)firstUpdate).StartedAt);
        }

        [Test]
        public unsafe void TestHarvestingComponentRemoved()
        {
            var tileResourceBefore = _logs.Get<TileResourceComponent>();
            _game.Network.DeltaCompression.ClearDeltas();
            _party.Logic.Harvesting.StartHarvesting(_logs);
            var totalTime = _logs.HarvestPointSpec.ResourceAmount * _logs.HarvestPointSpec.HarvestTimePerUnit;
            var startTime = _game.GameTime;

            // Advance all harvesting time
            _scheduler.SetLogicalTime(_game.GameTime + totalTime);
            _game.Network.DeltaCompression.ClearDeltas();
            var harvestedStack = _party.Logic.Harvesting.StopHarvesting();

            var entityUpdate = _party.Logic.DeltaCompression.GetUpdatePacket(_player.EntityId) as EntityUpdatePacket;

            Assert.IsTrue(entityUpdate.RemovedComponentIds.First() == Serialization.GetTypeId(typeof(HarvestingComponent)));
        }

        [Test]
        public void TestFinishHarvestingHalf()
        {
            var tileResourceBefore = _logs.Get<TileResourceComponent>().FastShallowClone();

            _party.Logic.Harvesting.StartHarvesting(_logs);
            var totalTime = _logs.HarvestPointSpec.ResourceAmount * _logs.HarvestPointSpec.HarvestTimePerUnit;

            // Advance half of the harvesting time
            _scheduler.SetLogicalTime(_game.GameTime + (totalTime / 2));

            var harvestedStack = _party.Logic.Harvesting.StopHarvesting();
            var tileResourceAfter = _logs.Get<TileResourceComponent>();
            var cargo = _party.Get<CargoComponent>();

            Assert.AreEqual(harvestedStack.Amount, tileResourceBefore.Resource.Amount / 2);
            Assert.AreEqual(tileResourceAfter.Resource.Amount, tileResourceBefore.Resource.Amount / 2);
            Assert.AreEqual(cargo.CurrentWeight, harvestedStack.Amount * _game.Specs.Resources[_logs.HarvestPointSpec.ResourceId].WeightPerUnit);

        }

        [Test]
        public void TestFinishHarvestingHalfTwice()
        {
            var tileResourceBefore = _logs.Get<TileResourceComponent>().FastShallowClone();

            _party.Logic.Harvesting.StartHarvesting(_logs);
            var totalTime = _logs.HarvestPointSpec.ResourceAmount * _logs.HarvestPointSpec.HarvestTimePerUnit;

            // Advance half of the harvesting time
            _scheduler.SetLogicalTime(_game.GameTime + (totalTime / 2));

            // Harvest the remaining half
            var firstHarvestedStack = _party.Logic.Harvesting.StopHarvesting();

            Assert.IsTrue(_party.Logic.Harvesting.StartHarvesting(_logs));
            _scheduler.SetLogicalTime(_game.GameTime + totalTime);
            var harvestedStack = _party.Logic.Harvesting.StopHarvesting();
            var cargo = _party.Get<CargoComponent>();

            var tileResourceAfter = _logs.Get<TileResourceComponent>();

            Assert.AreEqual(harvestedStack.Amount, tileResourceBefore.Resource.Amount / 2);
            Assert.AreEqual(0, tileResourceAfter.Resource.Amount);
            Assert.AreEqual(cargo.Slot1.Amount, tileResourceBefore.Resource.Amount);
            Assert.AreEqual(cargo.CurrentWeight, cargo.Slot1.Amount * _game.Specs.Resources[_logs.HarvestPointSpec.ResourceId].WeightPerUnit);
        }

        [Test]
        public unsafe void TestCourseToHarvest()
        {
            Assert.IsFalse(_party.Components.Has<HarvestingComponent>());

            // Course starts harvesting
            var ev = new MoveEntityCommand() { Path = new List<Location>() { _logs.Position }, Entity = _party.EntityId, Intent = CourseIntent.Harvest };
            ev.Sender = _player;
            _game.HandleClientEvent(_player, ev);
            _game.GameScheduler.Tick(_game.GameTime + _party.Course().Delay);

            var tileResources = _logs.Components.Get<TileResourceComponent>();

            Assert.IsTrue(_party.Components.Has<HarvestingComponent>());
            Assert.IsTrue(tileResources.BeingHarvested);

            // Course to end harvesting
            ev = new MoveEntityCommand() { Path = new List<Location>() { _logs.GetNeighbor(Direction.SOUTH).Position }, Entity = _party.EntityId };
            ev.Sender = _player;
            _game.HandleClientEvent(_player, ev);
            _game.GameScheduler.Tick(_game.GameTime + _party.Course().Delay);

            tileResources = _logs.Components.Get<TileResourceComponent>();

            Assert.IsFalse(_party.Components.Has<HarvestingComponent>());
            Assert.IsFalse(tileResources.BeingHarvested);
        }
    }
}