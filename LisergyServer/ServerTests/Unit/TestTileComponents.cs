using Game;
using Game.Dungeon;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Tile;
using NUnit.Framework;

namespace Tests
{
    public unsafe class TestTileComponents
    {

        private TileEntity tile1;
        private TileEntity tile2;

        [SetUp]
        public void Setup()
        {
            var c = new Chunk(null, 0, 0, new TileEntity[,] { });
            var t1 = new TileData();
            var t2 = new TileData();
            tile1 = new TileEntity(c, &t1, 0, 0);
            tile2 = new TileEntity(c, &t2, 1, 1);
        }

        [Test]
        public void TestEventsOnlyRegisteredOnce()
        {
            ComponentSet._buses.Clear();

            tile1.Components.Add(new TileHabitants());
            tile2.Components.Add(new TileHabitants());

            var tileBus = ComponentSet._buses[typeof(TileEntity)]._bus;

            Assert.AreEqual(1, tileBus._listeners.Count);
        }

        [Test]
        public void TestTileCallbacks()
        {
            tile1.Components.Add(new TileHabitants());
            tile2.Components.Add(new TileHabitants());

            var dg = new DungeonEntity();

            tile1.Components.CallEvent(new BuildingPlacedEvent(dg, tile1));

            Assert.IsTrue(tile1.Components.Get<TileHabitants>().Building == dg);
            Assert.IsTrue(tile2.Components.Get<TileHabitants>().Building == null);
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

            tile1.Components.CallEvent(new EntityMoveOutEvent() { Entity = new DungeonEntity() });

            Assert.IsTrue(TestView.called.Entity is DungeonEntity);

        }
    }
}