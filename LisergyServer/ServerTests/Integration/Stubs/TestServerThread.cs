using Game;
using GameDataTest.TestWorldGenerator;
using GameDataTest;
using MapServer;
using System;

using System.Threading;

namespace ServerTests.Integration.Stubs
{
    public class TestServerThread : IDisposable
    {
        public Thread Thread { get; private set; }
        public StandaloneServer Server { get; private set; }
        public LisergyGame Game { get; private set; }

        public TestServerThread()
        {
            Thread = new Thread(() =>
            {
                Game = new LisergyGame(TestSpecs.Generate());
                Game.SetupGame(new TestWorld(), new GameServerNetwork(Game));
                Server = new StandaloneServer(Game, 1337);
                Server.RunServer();
                if(Server.ServerError != null)
                {
                    throw Server.ServerError;
                }
            });
            Thread.Start();
        }

        public void Dispose()
        {
            Thread.Interrupt();
            Server?.Stop();
        }
    }
}
