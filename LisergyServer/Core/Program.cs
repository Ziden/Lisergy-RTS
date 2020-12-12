using Game;
using GameDataTest;
using LisergyServer.Core;

namespace LegendsServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = new GameConfiguration()
            {
                WorldMaxPlayers = 2
            };
            var gameSpecs = TestSpecs.Generate();
            var game = new StrategyGame(cfg, gameSpecs);
            game.GenerateMap();

            SocketServer.ServerLoop(game);
        }
    }
}
