using Game;
using GameDataTest.TestWorldGenerator;
using GameDataTest;
using MapServer;
using System;

using System.Threading;
using System.Collections.Generic;
using BaseServer.Core;

namespace ServerTests.Integration.Stubs
{
    public class RunningServer
    {
        public SocketServer Server;
        public Thread Thread;
    }

    /// <summary>
    /// Helper to allow a single machine to use multiple servers in different threads
    /// </summary>
    public class MultithreadServers : IDisposable
    {
        public Dictionary<ServerType, RunningServer> _running = new Dictionary<ServerType, RunningServer>();

        private void StartServer(SocketServer server)
        {
            var thread = new Thread(() =>
            {
                server.RunServer();
                if (server.ServerError != null) throw server.ServerError;
            });
            _running[server.GetServerType()] = new RunningServer()
            {
                Thread = thread,
                Server = server
            };
            thread.Start();
        }

        public MultithreadServers()
        {
            var game = new LisergyGame(TestSpecs.Generate(), new GameLog("[Server Game]"));
            game.SetupGame(new TestWorld(), new GameServerNetwork(game));
            StartServer(new WorldServer(game));
            StartServer(new AccountServer());
        }

        public void Dispose()
        {
            foreach (var running in _running)
            {
                running.Value.Server?.Stop();
                running.Value.Thread?.Interrupt();
            }
        }
    }
}
