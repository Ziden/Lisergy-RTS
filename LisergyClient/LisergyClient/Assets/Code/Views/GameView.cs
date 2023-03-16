using Assets.Code.Views;
using Assets.Code.World;
using Game;
using Game.ECS;
using Game.Events;
using Game.Events.Bus;
using GameData;
using System;

namespace Assets.Code
{
    public class GameView
    {
        private static StrategyGame _game;
        private static GameView _instance;
        private static ViewController _controller;

        public static TileView GetTileView(Tile tile)
        {
            var chunkView = GameView.GetView<ChunkView>(tile.Chunk);
            if (chunkView == null)
                GameView.Controller.AddView(tile.Chunk, new ChunkView(tile.Chunk));
            var tileView = GameView.GetView<TileView>(tile);
            if (tileView == null)
                tileView = GameView.Controller.AddView(tile, new TileView(tile));
            return tileView;
        }

        public static T GetView<T>(IEntity e) where T : IEntityView => _controller.GetView<T>(e);

        public static ClientWorld World => _game.World as ClientWorld;
        public static ViewController Controller => _controller;
        public static EventBus<GameEvent> Events => _game.GameEvents;

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
