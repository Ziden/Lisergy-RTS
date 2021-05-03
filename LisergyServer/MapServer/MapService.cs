using Game;
using Game.Events;
using Game.Generator;
using Game.Scheduler;
using GameDataTest;
using LisergyMessageQueue;
using LisergyServer.Commands;
using LisergyServer.Core;
using System;

namespace MapServer
{
    public class MapService : SocketServer
    {
        private static readonly int MAX_PLAYERS = 100;
     
        private static int WORLD_SEED = 12345;

        // TODO: Move to account server
        private AccountManager _accountManager;

        public override ServerType GetServerType() => ServerType.MAP;

        public MapService(int port) : base(port) {
        }

        public override void RegisterCommands(CommandExecutor executor)
        {
            executor.RegisterCommand(new TileCommand());
            executor.RegisterCommand(new TaskCommand());
        }

        public override void Tick()
        {
            GameScheduler.Tick(DateTime.UtcNow);
        }

        public override void Disconnect(int connectionID)
        {
            _accountManager.Disconnect(connectionID);
        }

        protected override ServerPlayer Auth(EventID eventId, int connectionID, byte[] message)
        {
            ServerPlayer caller;
            if (eventId != EventID.AUTH)
            {
                caller = _accountManager.GetPlayer(connectionID);
            }
            else
            {
                var ev = Serialization.ToEvent<AuthEvent>(message);
                ev.ConnectionID = connectionID;
                caller = _accountManager.Authenticate(ev);
            }
            if (caller == null)
            {
                Game.Log.Error($"Connection {connectionID} failed auth to send event {eventId.ToString()}");
                return null;
            }
            return caller;
        }

        public override StrategyGame SetupGame()
        {
            var gameSpecs = TestSpecs.Generate();
            var game = new StrategyGame(gameSpecs, null);
            GameWorld world = Worldgen.CreateWorld(MAX_PLAYERS, WORLD_SEED,
               new NewbieChunkPopulator()
              , new DungeonsPopulator()
            );
            game.World = world;
            game.RegisterEventListeners();
            _accountManager = new AccountManager(game, _socketServer);
            return game;
        }
    }
}
