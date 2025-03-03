using BaseServer.Core;
using BaseServer.Persistence;
using Game;
using Game.Engine;
using GameDataTest;
using GameDataTest.TestWorldGenerator;
using MapServer;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace ServerTests.Integration.Stubs
{
    /// <summary>
    /// Allows to run multiple servers in the same proccess using multiple threads.
    /// This way it makes it easier to debug and host all services in a single machine.
    /// </summary>
    public class StandaloneServer : IDisposable
    {
        public ConcurrentDictionary<ServerType, RunningServer> _servers = new ConcurrentDictionary<ServerType, RunningServer>();

        public bool Multithreaded = false;

        private static object _lock = new object();

        public FlatFileWorldPersistence Persistence;

        public LisergyGame Game;

        private void SetupServer(SocketServer server)
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

        public void SingleThreadTick()
        {
            if (Multithreaded) return;

            foreach (var instance in _servers.Values)
            {
                instance.Server.RunTick();
            }
        }

        public StandaloneServer Start()
        {

            foreach (var server in _servers.Values)
            {
                if (Multithreaded)
                {
                    server.Thread.Start();
                }
                else
                {
                    server.Server.StartListening().ContinueWith(c =>
                    {
                        c.Result.NoDelay = false;
                        c.Result.SendTimeout = 10;
                    }).Wait();
                }

            }

            return this;
        }

        public void BlockThread()
        {
            _servers.Values.First().Thread.Join();
        }

        public T GetInstance<T>(ServerType serverType) where T : SocketServer => (T)_servers[serverType].Server;

        public void SaveWorld(string worldname)
        {
            Persistence?.Save(GetInstance<WorldServer>(ServerType.WORLD).Game, worldname).Wait();
        }

        public StandaloneServer()
        {

            Serialization.LoadSerializers();
            var log = new GameLog("[Server Game]");
            log.Info("Starting standalone");
            Game = new LisergyGame(TestSpecs.Generate(), log);
            Game.SetupWorld(new TestWorld(Game));
            //Persistence = new FlatFileWorldPersistence(game.Log);

            SetupServer(new WorldServer(Game));
            SetupServer(new AccountServer());
            SetupServer(new ChatServer());
            Game.Log.Info("Game setup complete");

        }

        public void Dispose()
        {

            foreach (var running in _servers)
            {
                running.Value.Server?.Stop();
                running.Value.Thread?.Interrupt();
            }

        }
    }

    public class RunningServer
    {
        public SocketServer Server;
        public Thread Thread;
    }
}
