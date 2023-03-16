using Game;
using Game.ECS;
using Game.Entity;
using Game.Events;
using Game.Events.GameEvents;
using Game.World;
using Game.World.Components;
using Game.World.Data;
using GameDataTest;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace Tests
{
    public unsafe class TestTileComponents
    {

        private Tile tile1;
        private Tile tile2;

        [SetUp]
        public void Setup()
        {
            var c = new Chunk();
            var t1 = new TileData();
            var t2 = new TileData();
            tile1 = new Tile(ref c, ref t1, 0, 0);
            tile2 = new Tile(ref c, ref t2, 1, 1);
        }

        [Test]
        public void TestEventsOnlyRegisteredOnce()
        {
            ComponentSet<Tile>._buses.Clear();
            ComponentSet<WorldEntity>._buses.Clear();

            tile1.AddComponent<EntityPlacementComponent>();
            tile2.AddComponent<EntityPlacementComponent>();

            var tileBus = ComponentSet<Tile>._buses[typeof(Tile)]._bus;

            Assert.AreEqual(1, tileBus._listeners.Count);
        }

        [Test]
        public void TestTileCallbacks()
        {
            tile1.AddComponent<EntityPlacementComponent>();
            tile2.AddComponent<EntityPlacementComponent>();

            var dg = new Dungeon();

            tile1.CallComponentEvents(new StaticEntityPlacedEvent(dg, tile1));

            Assert.IsTrue(tile1.GetComponent<EntityPlacementComponent>().StaticEntity == dg);
            Assert.IsTrue(tile2.GetComponent<EntityPlacementComponent>().StaticEntity == null);


        }
    }
}