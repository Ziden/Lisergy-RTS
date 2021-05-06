using Game;
using Game.Events;
using LisergyServer.Core;
using MapServer;

namespace StandaloneServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var mapServer = new MapService(1337);
            mapServer.RunServer();
        }
    }
}
