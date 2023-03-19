using Assets.Code.Views;
using Assets.Code.World;
using Game;
using Game.ECS;
using Game.Entity;
using Game.Events;
using Game.Events.Bus;
using GameData;
using System;
using System.Diagnostics;

namespace Assets.Code
{
    public class GameView
    {
        private static StrategyGame _game;
        private static GameView _instance;
        private static ViewController _controller;

        public static TileView GetOrCreateTileView(Tile tile, bool ensureInstantiated = false)
        {
            var chunkView = GetView<ChunkView>(tile.Chunk);
            if (chunkView == null)
                Controller.AddView(tile.Chunk, new ChunkView(ref tile.Chunk));
            var tileView = GetView<TileView>(tile);
            if (tileView == null)
            {
                tileView = Controller.AddView(tile, new TileView(tile));
                Log.Debug($"Created tile view for {tile}");
            }
            if (ensureInstantiated)
            {
                tileView.Instantiate();
            }
            return tileView;
        }

        public static void Destroy(IEntityView view)
        {
            UnityEngine.Object.Destroy(view.GameObject);
            Controller.RemoveView(view);
        }

        public static T GetView<T>(IEntity e) where T : IEntityView => _controller.GetView<T>(e);

        public static IEntityView GetView(IEntity e) => _controller.GetView(e);

        public static ClientWorld World => _game.World as ClientWorld;
        public static ViewController Controller => _controller;

        public GameView(GameSpec specs, GameWorld world)
        {
            if(_instance != null)
            {
                throw new Exception("Trying to instantiate two games");
            }
            _controller = new ViewController();
            _instance = this;
            _game = new StrategyGame(specs, world);
        }
    }
}
