using Game;
using Game.Events;
using Game.Scheduler;
using GameDataTest;
using LisergyServer.Commands;
using LisergyServer.Core;
using System;

namespace MapServer
{
    public class MapService : SocketServer
    {
        private AccountManager _accountManager;

        public MapService()
        {
            _accountManager = new AccountManager(_socketServer);
        }

        public override ServerType GetServerType() => ServerType.MAP;

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
            var cfg = new GameConfiguration()
            {
                WorldMaxPlayers = 10
            };
            var gameSpecs = TestSpecs.Generate();
            var world = new GameWorld();
            var game = new StrategyGame(cfg, gameSpecs, world);
            game.GenerateMap();
            game.RegisterEventListeners();
            return game;
        }
    }
}
