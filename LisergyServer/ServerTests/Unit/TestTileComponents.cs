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
            var c = new Chunk(null, 0, 0, new Tile[,] { });
            var t1 = new TileData();
            var t2 = new TileData();
            tile1 = new Tile(c, &t1, 0, 0);
            tile2 = new Tile(c, &t2, 1, 1);
        }

        [Test]
        public void TestEventsOnlyRegisteredOnce()
        {
            ComponentSet<Tile>._buses.Clear();
            ComponentSet<WorldEntity>._buses.Clear();

            tile1.Components.Add(new TileHabitants());
            tile2.Components.Add(new TileHabitants());

            var tileBus = ComponentSet<Tile>._buses[typeof(Tile)]._bus;

            Assert.AreEqual(1, tileBus._listeners.Count);
        }

        [Test]
        public void TestTileCallbacks()
        {
            tile1.Components.Add(new TileHabitants());
            tile2.Components.Add(new TileHabitants());

            var dg = new Dungeon();

            tile1.Components.CallEvent(new StaticEntityPlacedEvent(dg, tile1));

            Assert.IsTrue(tile1.Components.Get<TileHabitants>().StaticEntity == dg);
            Assert.IsTrue(tile2.Components.Get<TileHabitants>().StaticEntity == null);
        }

        public class TestView : IComponent
        {
            public static EntityMoveOutEvent called = null;

            public static void Callback(TestView view, EntityMoveOutEvent ev) 
            {
                TestView.called = ev;
            }
    }

        [Test]
        public void TestViewEvents()
        {
            tile1.Components.Add(new TestView());

            
            tile1._components.RegisterExternalComponentEvents<TestView, EntityMoveOutEvent>(TestView.Callback);

            tile1.Components.CallEvent(new EntityMoveOutEvent() { Entity = new Dungeon() });

            Assert.IsTrue(TestView.called.Entity is Dungeon);

        }
    }
}