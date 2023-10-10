using Game;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Systems.Dungeon;
using Game.Systems.Tile;
using Game.Tile;
using Game.World;
using NUnit.Framework;
using ServerTests;

namespace Tests
{
    public unsafe class TestTileComponents
    {
        private IGame _game;
        private TileEntity tile1;
        private TileEntity tile2;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            tile1 = _game.GameWorld.GetTile(0, 0);
            tile2 = _game.GameWorld.GetTile(1, 1);
        }

        [Test]
        public void TestTileCallbacks()
        {
            tile1.Components.AddReference(new TileHabitants());
            tile2.Components.AddReference(new TileHabitants());

            var dg = _game.Entities.CreateEntity<DungeonEntity>(null);

            tile1.Components.CallEvent(new BuildingPlacedEvent(dg, tile1));

            Assert.IsTrue(tile1.Components.GetReference<TileHabitants>().Building == dg);
            Assert.IsTrue(tile2.Components.GetReference<TileHabitants>().Building == null);
        }

        public class TestView : IComponent
        {
            public static EntityMoveOutEvent called = default;

            public static void Callback(TestView view, EntityMoveOutEvent ev)
            {
                TestView.called = ev;
            }
        }

        /*
        [Test]
        public void TestViewEvents()
        {
            tile1.Components.Add(new TestView());


            //tile1._components.RegisterExternalComponentEvents<TestView, EntityMoveOutEvent>(TestView.Callback);

            tile1.Components.CallEvent(new EntityMoveOutEvent() { Entity = new DungeonEntity() });

            Assert.IsTrue(TestView.called.Entity is DungeonEntity);
        }
        */
    }
}