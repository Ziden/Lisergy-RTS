using Game;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Systems.Battler;
using Game.Systems.FogOfWar;
using Game.Systems.Tile;
using Game.World;
using GameDataTest;
using NUnit.Framework;
using ServerTests;
using System.Linq;
using System.Xml.Linq;

namespace Tests
{
    public class TestLineOfSight
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame(createPlayer: false);
        }

        [Test]
        public void TestLOSEvents()
        {
            Game.CreatePlayer();
            var initialBuildingSpec = Game.Specs.InitialBuilding;
            var player = Game.GetTestPlayer();
            var events = Game.SentPackets
                .Where(e => e is TilePacket)
                .Select(e => (TilePacket)e)
                .ToList();

            var range = initialBuildingSpec.LOS * 2 + 1;
            Assert.That(events.Count == range * range);
        }

        [Test]
        public void TestCreatingExploringEntityLOS()
        {
            var player = new TestServerPlayer(Game);
            var newbieChunk = Game.World.Map.GetUnnocupiedNewbieChunk();
            Game.World._worldPlayers.Add(player);
            var tile = newbieChunk.FindTileWithId(0);
            var castleID = Game.Specs.InitialBuilding.Id;
            player.EntityLogic.Player.Build(castleID, tile);

            Game.Entities.DeltaCompression.SendDeltaPackets(player);

            var los = Game.Specs.InitialBuilding.LOS;
            var losTiles = tile.GetAOE(los);
            var entityUpdates = player.ReceivedPacketsOfType<EntityUpdatePacket>();
            var tileVisibleEvent = player.ReceivedPacketsOfType<TilePacket>();

            Assert.AreEqual(1, entityUpdates.Count);
            Assert.AreEqual(losTiles.Count(), tileVisibleEvent.Count);
        }

        [Test]
        public void TestInitialLOS()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();

            var initialBuildingSpec = Game.Specs.InitialBuilding;
            var building = player.Data.Buildings.FirstOrDefault();
            var tile = building.Tile;
            var areaTiles = tile.GetAOE(initialBuildingSpec.LOS).ToList();

            Assert.AreEqual(player.Data.VisibleTiles.Count, areaTiles.Count());

            foreach (var seenTile in areaTiles)
            {
                Assert.True(seenTile.PlayersViewing.Contains(player));
                Assert.True(seenTile.EntitiesViewing.Contains(building));
                Assert.True(player.Data.VisibleTiles.Contains(seenTile));
            }
        }

        [Test]
        public void TestBuildingLineOfSight()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();

            var initialBuildingSpec = Game.Specs.InitialBuilding;
            var building = player.Data.Buildings.FirstOrDefault();
            var tile = building.Tile;
            var areaTiles = tile.GetAOE(initialBuildingSpec.LOS).ToList();

            Assert.AreEqual(player.Data.VisibleTiles.Count, areaTiles.Count());

            foreach (var seenTile in areaTiles)
            {
                Assert.True(seenTile.PlayersViewing.Contains(player));
                Assert.True(seenTile.EntitiesViewing.Contains(building));
                Assert.True(player.Data.VisibleTiles.Contains(seenTile));
            }
        }

        [Test]
        public void TestPartyLineOfSight()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();
            var party = player.Data.Parties.First();

            var los = party.Components.Get<EntityVisionComponent>().LineOfSight;

            Assert.AreEqual(los, Game.Specs.Units[party.Get<BattleGroupComponent>().Units[0].SpecId].LOS);
            Assert.AreEqual(los, party.Get<BattleGroupComponent>().Units.Array.Max(u => Game.Specs.Units[u.SpecId].LOS));
        }

        [Test]
        public void TestPartyUpdateLineOfSightOnRemoved()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();
            var party = player.Data.Parties.First();

            var logic = Game.Systems.BattleGroup.GetLogic(party);
            logic.RemoveUnit(party.Get<BattleGroupComponent>().Units[0]);

            var los = party.Components.Get<EntityVisionComponent>().LineOfSight;

            Assert.AreEqual(los, 0);
        }

        [Test]
        public void TestPartyUpdateLineOfSightOnAdded()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();
            var party = player.Data.Parties.First();

            var logic = Game.Systems.BattleGroup.GetLogic(party);

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

            Assert.That(los.Count() == range * range);
        }

        [Test]
        public void TestNotSendingEventsWhenNotReallyMoving()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();
            player.ReceivedPackets.Clear();

            var party = player.GetParty(0);
            Game.Logic.Map(party).SetPosition(party.Tile);

            Assert.AreEqual(0, player.ReceivedPackets.Count);
        }

        [Test]
        public void TestSendingEventsWhenExploring()
        {
            Game.Specs.InitialUnitSpecId = TestUnitData.MAGE;
            Game.CreatePlayer(0, 0); // placing new user in the corner
            var player = Game.GetTestPlayer();
            player.ListenTo<TileVisibilityChangedEvent>();
            player.ListenTo<TileExplorationStateChanged>();
            player.ReceivedPackets.Clear();
            var party = player.GetParty(0);

            Game.EntityLogic(party).Map.SetPosition(party.Tile.GetNeighbor(Direction.EAST));

            Game.Entities.DeltaCompression.SendDeltaPackets(player);

            /*          
                                o o o o E
                                o o o o E   
                                o o o o E    <- Explores this new row
            P Moving right ->   P o o o E 
              
            */

            // Only send update to visible ones
            Assert.AreEqual(party.GetLineOfSight() + 1, player.ReceivedPacketsOfType<TilePacket>().Count);
            Assert.AreEqual(party.GetLineOfSight() + 1, player.TriggeredEventsOfType<TileVisibilityChangedEvent>().Count);
            Assert.AreEqual(party.GetLineOfSight() + 1, player.TriggeredEventsOfType<TileExplorationStateChanged>().Count);
        }

        [Test]
        public void TestKeepingOneEntityExploring()
        {
            Game.Specs.InitialUnitSpecId = TestUnitData.KNIGHT;
            Game.CreatePlayer(0, 0); // placing new user in the corner
            var player = Game.GetTestPlayer();
            player.ListenTo<TileVisibilityChangedEvent>();
            player.ReceivedPackets.Clear();
            var party = player.GetParty(0); // 0-1
            var building = player.Data.Buildings.First(); // 0-0

            Assert.AreEqual(party.GetLineOfSight(), 1);

            Game.Logic.Map(party).SetPosition(party.Tile.GetNeighbor(Direction.EAST));

            /*          
                                o o o o o        o o o o o
                                o o o o o        o o o o o
                                E E E o o    ->  E E E E o  
            P Moving right ->   B P E o o        B E P E o

            The TileEntity on top of B (Building) should be explored by the building now and not the party anymore.
            Should not trigger visibility changes
            */

            var tileTopOfBuilding = building.Tile.GetNeighbor(Direction.NORTH);

            var visChanges = player.TriggeredEventsOfType<TileVisibilityChangedEvent>();

            Assert.IsFalse(visChanges.Any(ev => !ev.Visible && ev.Tile == tileTopOfBuilding));
            Assert.IsTrue(!tileTopOfBuilding.EntitiesViewing.Contains(party), "Party is not seeing the tile");
            Assert.IsTrue(tileTopOfBuilding.EntitiesViewing.Contains(building), "Building still seeing the tile");
        }


        [Test]
        public void TestLineOfSightChanges()
        {
            Game.CreatePlayer(0, 0);
            var player = Game.GetTestPlayer();
            var party = player.GetParty(0);
            var initialLos = party.GetLineOfSight();

            var lowerLosUnit = Game.Specs.Units.Where(kp => kp.Value.LOS < initialLos).First();
            var logic = Game.Systems.BattleGroup.GetLogic(party);
            logic.RemoveUnit(party.Get<BattleGroupComponent>().Units[0]);
            logic.AddUnit(new Unit(Game.Specs.Units[lowerLosUnit.Key]));

            var afterLos = party.GetLineOfSight();

            Assert.AreEqual(Game.Specs.InitialUnit.LOS, initialLos);
            Assert.AreEqual(lowerLosUnit.Value.LOS, afterLos);
            Assert.IsTrue(afterLos < initialLos);
        }

        [Test]
        public void TestNotSendingVisibleEventsWhenAlreadyExplored()
        {
            Game.CreatePlayer(0, 0);
            var player = Game.GetTestPlayer();
            var party = player.GetParty(0);
            Game.Logic.Map(party).SetPosition(party.Tile.GetNeighbor(Direction.EAST));

            player.ReceivedPackets.Clear();

            Game.Logic.Map(party).SetPosition(party.Tile.GetNeighbor(Direction.WEST));

            Assert.AreEqual(0, player.ReceivedPacketsOfType<TilePacket>().Count);
        }
    }
}