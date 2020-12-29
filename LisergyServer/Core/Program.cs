using Game;
using GameDataTest;
using LisergyServer;
using LisergyServer.Core;

namespace LegendsServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = new GameConfiguration()
            {
                WorldMaxPlayers = 10
            };
            var gameSpecs = TestSpecs.Generate();
            var world = new GameWorld();
            var game = new StrategyGame(cfg, gameSpecs, world);
            game.LoadMap();
            new ServerWorldListener(world);
            SocketServer.RunGame(game);
        }
    }
}
