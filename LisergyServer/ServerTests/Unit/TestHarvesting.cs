using NUnit.Framework;
using ServerTests;
using Game.Systems.Player;
using Game.Tile;
using Game.Systems.Tile;
using GameDataTest;
using Game.Systems.Party;
using Game.World;
using Game.Systems.Resources;
using Game.Scheduler;
using Game.Network.ClientPackets;
using System.Collections.Generic;
using Game.Systems.Movement;
using Game.Events.ServerEvents;
using System.Linq;
using System;
using Game;
using GameDataTest.TestWorldGenerator;

namespace UnitTests
{
    public class TestHarvesting
    {
        private TestGame _game;
        private TestServerPlayer _player;
        private PartyEntity _party;
        private TileEntity _logs;
        private GameScheduler _scheduler;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _party = _player.GetParty(0);
            _logs = _game.TestMap.GetTile(5, 5);
            _party.EntityLogic.Map.SetPosition(_logs.GetNeighbor(Direction.SOUTH));
            _scheduler = _game.Scheduler as GameScheduler;
            _logs.SpecId = TestTiles.FOREST.ID;
        }

        [Test]
        public void TestCanHarvest()
        {
            Assert.IsTrue(_party.EntityLogic.Harvesting.CanHarvest(_logs));
        }

        [Test]
        public void TestCannotHarvestIfFar()
        {
            _party.EntityLogic.Map.SetPosition(_party.Tile.GetNeighbor(Direction.SOUTH));

            Assert.IsFalse(_party.EntityLogic.Harvesting.CanHarvest(_logs));
        }

        [Test]
        public void TestBeginHarvesting()
        {
            var harvesting = _party.EntityLogic.Harvesting.StartHarvesting(_logs);

            Assert.IsTrue(harvesting);

            var harvestingComponent = _party.Get<HarvestingComponent>();
            var tileResource = _logs.Get<TileResourceComponent>();

            Assert.AreEqual(true, tileResource.BeingHarvested);
            Assert.AreEqual(_logs.Position, harvestingComponent.Tile);
            Assert.AreEqual(_game.GameTime.ToBinary(), harvestingComponent.StartedAt);
        }

        [Test]
        public void TestFinishHarvestingAll()
        {
            var tileResourceBefore = _logs.Get<TileResourceComponent>();

            _party.EntityLogic.Harvesting.StartHarvesting(_logs);
            var totalTime = _logs.HarvestPointSpec.ResourceAmount * _logs.HarvestPointSpec.HarvestTimePerUnit;

            // Advance all harvesting time
            _scheduler.SetLogicalTime(_game.GameTime + totalTime);

            var harvestedStack = _party.EntityLogic.Harvesting.StopHarvesting();

            var tileResourceAfter = _logs.Get<TileResourceComponent>();
            var resourcedSubtracted = tileResourceBefore.AmountResourcesLeft - tileResourceAfter.AmountResourcesLeft;
            var partyCargo = _party.Get<CargoComponent>();

            Assert.AreEqual(resourcedSubtracted, partyCargo.GetAmount(tileResourceAfter.ResourceId));
            Assert.AreEqual(resourcedSubtracted, harvestedStack.Amount);
            Assert.IsFalse(_party.Components.Has<HarvestingComponent>());
            Assert.AreEqual(false, tileResourceAfter.BeingHarvested);
        }

        [Test]
        public void TestFinishHarvestingPackets()
        {
            var tileResourceBefore = _logs.Get<TileResourceComponent>();

            _party.EntityLogic.Harvesting.StartHarvesting(_logs);
            var totalTime = _logs.HarvestPointSpec.ResourceAmount * _logs.HarvestPointSpec.HarvestTimePerUnit;
            _scheduler.SetLogicalTime(_game.GameTime + totalTime);

            _game.Entities.DeltaCompression.ClearDeltas();
            _player.ReceivedPackets.Clear();

            var harvestedStack = _party.EntityLogic.Harvesting.StopHarvesting();
            _game.Entities.DeltaCompression.SendDeltaPackets(_player);

            var tileUpdate = _player.ReceivedPacketsOfType<TileUpdatePacket>().First();

            Assert.That(tileUpdate.Components.Any(c => c.GetType() == typeof(TileResourceComponent)));
            
        }

        [Test]
        public unsafe void TestHarvestingComponentSync()
        {
            var tileResourceBefore = _logs.Get<TileResourceComponent>();
            _game.Entities.DeltaCompression.ClearDeltas();
            _party.EntityLogic.Harvesting.StartHarvesting(_logs);
            var totalTime = _logs.HarvestPointSpec.ResourceAmount * _logs.HarvestPointSpec.HarvestTimePerUnit;
            var startTime = _game.GameTime;

            var entityUpdate1 = _party.GetUpdatePacket(_player) as EntityUpdatePacket;

            // Advance all harvesting time
            _scheduler.SetLogicalTime(_game.GameTime + totalTime);
            _game.Entities.DeltaCompression.ClearDeltas();
            var harvestedStack = _party.EntityLogic.Harvesting.StopHarvesting();

            var entityUpdate2 = _party.GetUpdatePacket(_player) as EntityUpdatePacket;

            var firstUpdate = (HarvestingComponent)entityUpdate1.SyncedComponents.FirstOrDefault(c => c.GetType() == typeof(HarvestingComponent));
            var secondUpdate = (HarvestingComponent)entityUpdate2.SyncedComponents.FirstOrDefault(c => c.GetType() == typeof(HarvestingComponent));

            Assert.AreEqual(startTime.ToBinary(), firstUpdate.StartedAt);
            Assert.AreEqual(DateTime.MinValue.ToBinary(), secondUpdate.StartedAt);
        }

        [Test]
        public void TestFinishHarvestingHalf()
        {
            var tileResourceBefore = _logs.Get<TileResourceComponent>();

            _party.EntityLogic.Harvesting.StartHarvesting(_logs);
            var totalTime = _logs.HarvestPointSpec.ResourceAmount * _logs.HarvestPointSpec.HarvestTimePerUnit;

            // Advance half of the harvesting time
            _scheduler.SetLogicalTime(_game.GameTime + (totalTime /2));

            var harvestedStack = _party.EntityLogic.Harvesting.StopHarvesting();
            var tileResourceAfter = _logs.Get<TileResourceComponent>();

            Assert.AreEqual(harvestedStack.Amount, tileResourceBefore.AmountResourcesLeft / 2);
            Assert.AreEqual(tileResourceAfter.AmountResourcesLeft, tileResourceBefore.AmountResourcesLeft / 2);
        }

        [Test]
        public unsafe void TestCourseToHarvest()
        {
            Assert.IsFalse(_party.Components.Has<HarvestingComponent>());

            // Course starts harvesting
            var ev = new MoveRequestPacket() { Path = new List<TileVector>() { _logs.Position }, PartyIndex = _party.PartyIndex, Intent = CourseIntent.Harvest };
            ev.Sender = _player;
            _game.HandleClientEvent(_player, ev);
            _game.GameScheduler.Tick(_game.GameTime + _party.Course.Delay);

            var tileResources = _logs.Components.GetPointer<TileResourceComponent>();

            Assert.IsTrue(_party.Components.Has<HarvestingComponent>());
            Assert.IsTrue(tileResources->BeingHarvested);

            // Course to end harvesting
            ev = new MoveRequestPacket() { Path = new List<TileVector>() { _logs.GetNeighbor(Direction.SOUTH).Position }, PartyIndex = _party.PartyIndex };
            ev.Sender = _player;
            _game.HandleClientEvent(_player, ev);
            _game.GameScheduler.Tick(_game.GameTime + _party.Course.Delay);

            Assert.IsFalse(_party.Components.Has<HarvestingComponent>());
            Assert.IsFalse(tileResources->BeingHarvested);
        }
    }
}