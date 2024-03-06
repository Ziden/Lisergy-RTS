using Game;
using GameDataTest.TestWorldGenerator;
using GameDataTest;
using MapServer;
using System;
using System.Threading;
using BaseServer.Core;
using System.Collections.Concurrent;
using System.Linq;
using BaseServer.Persistence;
using Game.Engine;

namespace ServerTests.Integration.Stubs
{
    /// <summary>
    /// Allows to run multiple servers in the same proccess using multiple threads.
    /// This way it makes it easier to debug and host all services in a single machine.
    /// </summary>
    public class StandaloneServer : IDisposable
    {
        public ConcurrentDictionary<ServerType, RunningServer> _servers = new ConcurrentDictionary<ServerType, RunningServer>();

        private static object _lock = new object();

        public FlatFileWorldPersistence Persistence;

        private void SetupServer(SocketServer server)
        {
            lock(_lock)
            {
                _servers[server.GetServerType()] = new RunningServer()
                {
                    Thread = new Thread(() =>
                    {
                        server.RunServer();
                        if (server.ServerError != null) throw server.ServerError;
                    }),
                    Server = server
                };
            }
           
        }

        public StandaloneServer Start()
        { 
            lock(_lock)
            {
                foreach (var server in _servers.Values)
                {
                    server.Thread.Start();
                }
            }
            return this;
        }

        public void BlockThread()
        {
            _servers.Values.First().Thread.Join();
        }

        public T GetInstance<T>(ServerType serverType) where T: SocketServer => (T)_servers[serverType].Server;

        public void SaveWorld(string worldname)
        {
            Persistence.Save(GetInstance<WorldServer>(ServerType.WORLD).Game, worldname).Wait();
        }

        public StandaloneServer()
        {
            lock(_lock)
            {
                var game = new LisergyGame(TestSpecs.Generate(), new GameLog("[Server Game]"));
                game.SetupWorld(new TestWorld(game));
                Persistence = new FlatFileWorldPersistence(game.Log);

                SetupServer(new WorldServer(game));
                SetupServer(new AccountServer());
                SetupServer(new ChatServer());
            } 
        }

        public void Dispose()
        {
            lock(_lock)
            {
                foreach (var running in _servers)
                {
                    running.Value.Server?.Stop();
                    running.Value.Thread?.Interrupt();
                }
            }
        }
    }

    public class RunningServer
    {
        public SocketServer Server;
        public Thread Thread;
    }
}
