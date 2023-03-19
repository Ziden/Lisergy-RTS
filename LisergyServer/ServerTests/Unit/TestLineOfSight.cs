using Game;
using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests;
using System.Linq;
using Game.World;
using Game.World.Components;
using GameDataTest;
using Game.Events.GameEvents;

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
            var initialBuildingSpec = StrategyGame.Specs.Buildings[StrategyGame.Specs.InitialBuilding];
            var player = Game.GetTestPlayer();
            var events = Game.ReceivedEvents
                .Where(e => e is TileUpdatePacket)
                .Select(e => (TileUpdatePacket)e)
                .ToList();

            var range = initialBuildingSpec.LOS * 2 + 1;
            Assert.That(events.Count == range * range);
        }

        [Test]
        public void TestCreatingExploringEntityLOS()
        {
            var player = new TestServerPlayer();
            var newbieChunk = Game.World.Map.GetUnnocupiedNewbieChunk();
            Game.World.Players.Add(player);
            var tile = newbieChunk.FindTileWithId(0);
            var castleID = StrategyGame.Specs.InitialBuilding;
            player.Build(castleID, tile);

            DeltaTracker.SendDeltaPackets(player);

            var los = StrategyGame.Specs.Buildings[castleID].LOS;
            var losTiles = tile.GetAOE(los);
            var entityUpdates = player.ReceivedEventsOfType<EntityUpdatePacket>();
            var tileVisibleEvent = player.ReceivedEventsOfType<TileUpdatePacket>();

            Assert.AreEqual(1, entityUpdates.Count);
            Assert.AreEqual(losTiles.Count(), tileVisibleEvent.Count);
        }

        [Test]
        public void TestInitialLOS()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();

            var initialBuildingSpec = StrategyGame.Specs.Buildings[StrategyGame.Specs.InitialBuilding];
            var building = player.Buildings.FirstOrDefault();
            var tile = building.Tile;
            var areaTiles = tile.GetAOE(initialBuildingSpec.LOS).ToList();

            Assert.AreEqual(player.VisibleTiles.Count, areaTiles.Count());

            foreach (var seenTile in areaTiles)
            {
                Assert.True(seenTile.Components.Get<TileVisibility>().PlayersViewing.Contains(player));
                Assert.True(seenTile.Components.Get<TileVisibility>().EntitiesViewing.Contains(building));
                Assert.True(player.VisibleTiles.Contains(seenTile));
            }
        }

        [Test]
        public void TestBuildingLineOfSight()
        {
            Game.CreatePlayer();
            var player = Game.GetTestPlayer();

            var initialBuildingSpec = StrategyGame.Specs.Buildings[StrategyGame.Specs.InitialBuilding];
            var building = player.Buildings.FirstOrDefault();
            var tile = building.Tile;
            var areaTiles = tile.GetAOE(initialBuildingSpec.LOS).ToList();

            Assert.AreEqual(player.VisibleTiles.Count, areaTiles.Count());

            foreach (var seenTile in areaTiles)
            {
                Assert.True(seenTile.Components.Get<TileVisibility>().PlayersViewing.Contains(player));
                Assert.True(seenTile.Components.Get<TileVisibility>().EntitiesViewing.Contains(building));
                Assert.True(player.VisibleTiles.Contains(seenTile));
            }
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
            player.ReceivedEvents.Clear();

            var party = player.GetParty(0);

            party.Tile = party.Tile;

            Assert.AreEqual(0, player.ReceivedEvents.Count);
        }

        [Test]
        public void TestSendingEventsWhenExploring()
        {
            StrategyGame.Specs.InitialUnit = 2; // Mage with 3+ LOS
            Game.CreatePlayer(0, 0); // placing new user in the corner
            var player = Game.GetTestPlayer();
            player.ListenTo<TileVisibilityChangedEvent>();
            player.ListenTo<TileExplorationStateChanged>();
            player.ReceivedEvents.Clear();
            var party = player.GetParty(0);

            party.Tile = party.Tile.GetNeighbor(Direction.EAST);
            DeltaTracker.SendDeltaPackets(player);

            /*          
                                o o o o E
                                o o o o E   
                                o o o o E    <- Explores this new row
            P Moving right ->   P o o o E 
              
            */

            // Only send update to visible ones
            Assert.AreEqual(party.GetLineOfSight() + 1, player.ReceivedEventsOfType<TileUpdatePacket>().Count);
            Assert.AreEqual(party.GetLineOfSight() + 1, player.ReceivedEventsOfType<TileVisibilityChangedEvent>().Count);
            Assert.AreEqual(party.GetLineOfSight() + 1, player.ReceivedEventsOfType<TileExplorationStateChanged>().Count);
        }

        [Test]
        public void TestKeepingOneEntityExploring()
        {
            StrategyGame.Specs.InitialUnit = TestUnitData.KNIGHT; // Thief with 1 los
            Game.CreatePlayer(0, 0); // placing new user in the corner
            var player = Game.GetTestPlayer();
            player.ListenTo<TileVisibilityChangedEvent>();
            player.ReceivedEvents.Clear();
            var party = player.GetParty(0); // 0-1
            var building = player.Buildings.First(); // 0-0

            Assert.AreEqual(party.GetLineOfSight(), 1);
     
            party.Tile = party.Tile.GetNeighbor(Direction.EAST);
            /*          
                                o o o o o        o o o o o
                                o o o o o        o o o o o
                                E E E o o    ->  E E E E o  
            P Moving right ->   B P E o o        B E P E o

            The tile on top of B (Building) should be explored by the building now and not the party anymore.
            Should not trigger visibility changes
            */

            var tileTopOfBuilding = building.Tile.GetNeighbor(Direction.NORTH);

            var visChanges = player.ReceivedEventsOfType<TileVisibilityChangedEvent>();

            Assert.IsFalse(visChanges.Any(ev => !ev.Visible && ev.Tile == tileTopOfBuilding));
            Assert.IsTrue(!tileTopOfBuilding.Components.Get<TileVisibility>().EntitiesViewing.Contains(party), "Party is not seeing the tile");
            Assert.IsTrue(tileTopOfBuilding.Components.Get<TileVisibility>().EntitiesViewing.Contains(building), "Building still seeing the tile");
        }


        [Test]
        public void TestLineOfSightChanges()
        {
            Game.CreatePlayer(0, 0);
            var player = Game.GetTestPlayer();
            var party = player.GetParty(0);
            var initialLos = party.GetLineOfSight();

            var lowerLosUnit = Game.GameSpec.Units.Where(kp => kp.Value.LOS < initialLos).First();

            party.BattleLogic.RemoveUnit(party.BattleLogic.GetUnits().First());
            party.BattleLogic.AddUnit(new Unit(lowerLosUnit.Key));

            var afterLos = party.GetLineOfSight();

            Assert.AreEqual(Game.GameSpec.Units[Game.GameSpec.InitialUnit].LOS, initialLos);
            Assert.AreEqual(lowerLosUnit.Value.LOS, afterLos);
            Assert.IsTrue(afterLos < initialLos);
        }

        [Test]
        public void TestNotSendingVisibleEventsWhenAlreadyExplored()
        {
            Game.CreatePlayer(0, 0);
            var player = Game.GetTestPlayer();
            var party = player.GetParty(0);
            party.Tile = party.Tile.GetNeighbor(Direction.EAST);
            player.ReceivedEvents.Clear();

            party.Tile = party.Tile.GetNeighbor(Direction.WEST);

            Assert.AreEqual(0, player.ReceivedEventsOfType<TileUpdatePacket>().Count);
        }
    }
}