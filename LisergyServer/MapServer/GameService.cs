using Game;
using Game.BlockChain;
using Game.Events;
using Game.Scheduler;
using LisergyServer.Commands;
using LisergyServer.Core;
using System;

namespace MapServer
{
    public class GameService : SocketServer
    {
        // TODO: Move to account server
        private AuthManager _accountManager;

        public override ServerType GetServerType() => ServerType.MAP;

        public GameService(int port) : base(port) {
        }

        public override void RegisterCommands(BlockchainGame game, CommandExecutor executor)
        {
            executor.RegisterCommand(new TileCommand(game));
            executor.RegisterCommand(new TaskCommand(game));
            executor.RegisterCommand(new BattlesCommand(game));
            executor.RegisterCommand(new ServerCommand(game));
        }

        public override void Tick()
        {
            GameScheduler.Tick(DateTime.UtcNow);
        }

        public override void Disconnect(int connectionID)
        {
            _accountManager.Disconnect(connectionID);
        }

        protected override ServerPlayer Auth(BaseEvent ev, int connectionID)
        {
            ServerPlayer caller;
            if (!(ev is AuthPacket))
            {
                caller = _accountManager.GetPlayer(connectionID);
            }
            else
            {
                ev.ConnectionID = connectionID;
                caller = _accountManager.Authenticate((AuthPacket)ev);
            }
            if (caller == null)
            {
                Game.Log.Error($"Connection {connectionID} failed auth to send event {ev}");
                return null;
            }
            return caller;
        }

        public override BlockchainGame SetupGame()
        {
            var game = new BlockchainGame(new TestChain());
            game.RegisterEventListeners();
            _accountManager = new AuthManager(game, _socketServer);
            return game;
        }
    }
}
