using Game.Entities;
using Game.Events.ServerEvents;
using Game.Systems.Battler;
using Game.Systems.FogOfWar;
using Game.World;
using GameDataTest;
using NUnit.Framework;
using ServerTests;
using System.Linq;
using Tests.Unit.Stubs;

namespace GameUnitTests
{
    public class TestLineOfSight
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame(createPlayer: false);
        }

        [TearDown]
        public void TearDown()
        {
            TestGame.BuildTestSpecs();
        }

        [Test]
        public void TestLOSEvents()
        {
            Game.CreatePlayer();
            var initialBuildingSpec = Game.Specs.InitialBuilding;
            var player = Game.GetTestPlayer();
            var events = Game.SentServerPackets
                .Where(e => e is EntityUpdatePacket p && p.Type == EntityType.Tile)
                .Select(e => (EntityUpdatePacket)e)
                .ToList();

            var range = initialBuildingSpec.LOS * 2 + 1;
            Assert.AreEqual(range * range - 4, events.Count);
        }

        [Test]
        public void TestCreatingExploringEntityLOS()
        {
            var player = new TestServerPlayer(Game);
            var newbieChunk = Game.World.GetUnusedStartingTile().Chunk;
            Game.World.Players.Add(player);
            var tile = newbieChunk.FindTileWithId(0);
            var castleID = Game.Specs.InitialBuilding.SpecId;
            player.EntityLogic.Build(castleID, tile);

            Game.Network.DeltaCompression.SendAllModifiedEntities(player.EntityId);

            var los = Game.Specs.InitialBuilding.LOS;
            var losTiles = tile.GetAOE(los);
            var entityUpdates = player.ReceivedPacketsOfType<EntityUpdatePacket>().Where(p => p.Type != EntityType.Tile).ToList();
            var tileVisibleEvent = player.ReceivedPacketsOfType<EntityUpdatePacket>().Where(p => p.Type == EntityType.Tile).ToList();

            Assert.AreEqual(1, entityUpdates.Count);
            Assert.AreEqual(losTiles.Count(), tileVisibleEvent.Count);
        }

        [Test]
        public void TestInitialLOS()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();

            var initialBuildingSpec = Game.Specs.InitialBuilding;

            var building = player.Buildings.FirstOrDefault();
            var tile = building.GetTile();
            var areaTiles = tile.GetAOE(initialBuildingSpec.LOS).ToList();

            Assert.AreEqual(player.VisibilityData.VisibleTiles.Count, areaTiles.Count());

            foreach (var seenTile in areaTiles)
            {
                Assert.True(seenTile.Logic.Vision.GetPlayersViewing().Contains(player.EntityId));
                Assert.True(seenTile.Logic.Vision.GetEntitiesViewing().Contains(building.EntityId));
                Assert.True(player.VisibilityData.VisibleTiles.Contains(seenTile.Position));
            }
        }

        [Test]

        public void TestBuildingLineOfSight()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();

            var initialBuildingSpec = Game.Specs.InitialBuilding;
            var building = player.Buildings.FirstOrDefault();
            var tile = building.GetTile();
            var areaTiles = tile.GetAOE(initialBuildingSpec.LOS).ToList();

            Assert.AreEqual(player.VisibilityData.VisibleTiles.Count, areaTiles.Count());

            foreach (var seenTile in areaTiles)
            {
                Assert.True(seenTile.Logic.Vision.GetPlayersViewing().Contains(player.EntityId));
                Assert.True(seenTile.Logic.Vision.GetEntitiesViewing().Contains(building.EntityId));
                Assert.True(player.VisibilityData.VisibleTiles.Contains(seenTile.Position));
            }
        }

        [Test]
        public void TestPartyLineOfSight()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();
            var party = player.Parties.First();

            var los = party.Components.Get<EntityVisionComponent>().LineOfSight;

            Assert.AreEqual(los, Game.Specs.Units[party.Get<BattleGroupComponent>().Units[0].SpecId].LOS);
            Assert.AreEqual(los, party.Get<BattleGroupComponent>().Units.Max(u => Game.Specs.Units[u.SpecId].LOS));
        }

        [Test]
        public void TestPartyUpdateLineOfSightOnRemoved()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();
            var party = player.Parties.First();

            var logic = party.Logic.BattleGroup;
            logic.RemoveUnit(party.Get<BattleGroupComponent>().Units[0]);

            var los = party.Components.Get<EntityVisionComponent>().LineOfSight;

            Assert.AreEqual(0, los);
        }

        [Test]
        public void TestPartyUpdateLineOfSightOnAdded()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();
            var party = player.Parties.First();

            var logic = party.Logic.BattleGroup;

            logic.RemoveUnit(party.Get<BattleGroupComponent>().Units[0]);

            var unit = new Unit(Game.Specs.Units[0]);
            logic.AddUnit(unit);

            var los = party.Components.Get<EntityVisionComponent>().LineOfSight;

            Assert.AreEqual(los, Game.Specs.Units[unit.SpecId].LOS);
        }

        [Test]
        public void TestTileAOE()
        {
            var tile = Game.World.GetTile(5, 5);

            ushort aoe = 4;
            var range = aoe * 2 + 1;
            var los = tile.GetAOE(aoe).ToList();

            Assert.That(los.Count() == range * range - 4);
        }

        [Test]
        public void TestNotSendingEventsWhenNotReallyMoving()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();
            player.ReceivedPackets.Clear();

            var party = player.GetParty(0);
            party.Logic.Map.SetPosition(party.GetTile());

            Assert.AreEqual(0, player.ReceivedPackets.Count);
        }

        [Test]
        public void TestSendingEventsWhenExploring()
        {
            Game.Specs.InitialUnitSpecId = TestUnitData.MAGE;
            Game.CreatePlayer(0, 0); // placing new user in the corner
            var player = Game.GetTestPlayer();

            player.ReceivedPackets.Clear();
            player.TriggeredEvents.Clear();

            var party = player.GetParty(0);

            var oldTIle = party.GetTile();
            var newTile = oldTIle.GetNeighbor(Direction.EAST);
            party.Logic.Map.SetPosition(newTile);

            Game.Network.DeltaCompression.SendAllModifiedEntities(player.EntityId);

            /*          
                                o o o o X
                                o o o o E   
                                o o o o E    <- Explores this new row
            P Moving right ->   P o o o E 
              
            */

            // Only send update to visible ones
            Assert.AreEqual(party.GetLineOfSight(), player.ReceivedEntityUpdates(EntityType.Tile).Count);
            Assert.AreEqual(party.GetLineOfSight(), player.TriggeredEventsOfType<TileVisibilityChangedEvent>().Count);
        }

        [Test]
        public void TestKeepingOneEntityExploring()
        {
            Game.Specs.InitialUnitSpecId = TestUnitData.KNIGHT;
            Game.CreatePlayer(0, 0); // placing new user in the corner
            var player = Game.GetTestPlayer();
            player.ListenTo<TileVisibilityChangedEvent>();

            var party = player.GetParty(0); // 0-1
            var building = player.Buildings.First(); // 0-0

            Assert.AreEqual(1, party.GetLineOfSight());

            party.Logic.Map.SetPosition(party.GetTile().GetNeighbor(Direction.EAST));

            /*          
                                o o o o o        o o o o o
                                o o o o o        o o o o o
                                E E E o o    ->  E E E E o  
            P Moving right ->   B P E o o        B E P E o

            The TileEntity on top of B (Building) should be explored by the building now and not the party anymore.
            Should not trigger visibility changes
            */

            var tileTopOfBuilding = building.GetTile().GetNeighbor(Direction.NORTH);

            var visChanges = player.TriggeredEventsOfType<TileVisibilityChangedEvent>();

            Assert.IsFalse(visChanges.Any(ev => !ev.Visible && ev.Tile == tileTopOfBuilding));
            Assert.IsTrue(!tileTopOfBuilding.Logic.Vision.GetEntitiesViewing().Contains(party.EntityId), "Party is not seeing the tile");
            Assert.IsTrue(tileTopOfBuilding.Logic.Vision.GetEntitiesViewing().Contains(building.EntityId), "Building still seeing the tile");
        }


        [Test]
        public void TestLineOfSightChanges()
        {
            Game.Specs.InitialUnitSpecId = TestUnitData.MAGE;
            Game.CreatePlayer(0, 0);
            var player = Game.GetTestPlayer();
            var party = player.GetParty(0);
            var initialLos = party.GetLineOfSight();

            var lowerLosUnit = Game.Specs.Units.Where(kp => kp.Value.LOS < initialLos).First();
            var logic = party.Logic.BattleGroup;
            logic.RemoveUnit(party.Get<BattleGroupComponent>().Units[0]);
            logic.AddUnit(new Unit(Game.Specs.Units[lowerLosUnit.Key]));

            var afterLos = party.GetLineOfSight();

            Assert.AreEqual(Game.Specs.InitialUnit.LOS, initialLos);
            Assert.AreEqual(lowerLosUnit.Value.LOS, afterLos);
            Assert.IsTrue(afterLos < initialLos);
        }

        [Test]
        public void TestEventsAndPacketsSync()
        {
            var player = Game.CreatePlayer(4, 4);
            var party = player.GetParty(0);

            var los = party.GetLineOfSight();
            var visibleTiles = player.EntityLogic.GetVisibleTiles();
            var visibleEvents = player.TriggeredEventsOfType<TileVisibilityChangedEvent>();
            var tileUpdates = player.ReceivedPacketsOfType<EntityUpdatePacket>().Where(p => p.Type == EntityType.Tile).ToList();

            Assert.AreEqual(visibleTiles.Count, visibleEvents.Count);
            Assert.AreEqual(visibleTiles.Count, tileUpdates.Count);
            Assert.Greater(visibleTiles.Count, 0);
        }

        [Test]
        public void TestNotSendingVisibleEventsWhenAlreadyExplored()
        {
            Game.CreatePlayer(0, 0);
            var player = Game.GetTestPlayer();
            var party = player.GetParty(0);
            party.Logic.Map.SetPosition(party.GetTile().GetNeighbor(Direction.EAST));

            player.ReceivedPackets.Clear();

            party.Logic.Map.SetPosition(party.GetTile().GetNeighbor(Direction.WEST));

            Assert.AreEqual(0, player.ReceivedEntityUpdates(EntityType.Tile).Count);
        }
    }
}