using Assets.Code.Views;
using Assets.Code.World;
using Game;
using Game.ECS;
using Game.Tile;
using Game.World;
using GameData;
using System;

namespace Assets.Code
{
    public class GameView
    {
        
        private static LisergyGame _game;
        private static GameView _instance;
        private static ViewController _controller;

        public static TileView GetOrCreateTileView(TileEntity tile, bool ensureInstantiated = false)
        {
            return null;
            /*
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
              */
        }

        public static void Destroy(IEntityView view)
        {
            UnityEngine.Object.Destroy(view.GameObject);
            Controller.RemoveView(view);
        }

        public static bool Initialized => _instance != null;
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
            _game = new LisergyGame(specs);
            _game.SetupGame(world, new GameServerNetwork(_game));
        }
    }
}
