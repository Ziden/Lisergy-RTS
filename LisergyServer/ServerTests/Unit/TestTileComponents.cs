using Game;
using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Entities;
using Game.Systems.Map;
using Game.Systems.Tile;
using Game.Tile;
using NUnit.Framework;
using ServerTests;

namespace GameUnitTests
{
    public unsafe class TestTileComponents
    {
        private IGame _game;
        private TileModel tile1;
        private TileModel tile2;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            tile1 = _game.World.GetTile(0, 0);
            tile2 = _game.World.GetTile(1, 1);
        }

        [Test]
        public void TestTileCallbacks()
        {
            tile1.Components.Add<TileHabitantsComponent>();
            tile2.Components.Add<TileHabitantsComponent>();

            var dg = _game.Entities.CreateEntity(EntityType.Dungeon);

            dg.Components.CallEvent(new ComponentUpdateEvent<MapPlacementComponent>()
            {
                Entity = dg,
                New = new MapPlacementComponent()
                {
                    Position = tile1.Position
                },
            });

            Assert.IsTrue(tile1.Components.Get<TileHabitantsComponent>().Building == dg);
            Assert.IsTrue(tile2.Components.Get<TileHabitantsComponent>().Building == null);
        }

        public class TestView : IComponent
        {
            public static ComponentUpdateEvent<MapPlacementComponent> called = default;

            public static void Callback(TestView view, ComponentUpdateEvent<MapPlacementComponent> ev)
            {
                TestView.called = ev;
            }
        }
    }
}