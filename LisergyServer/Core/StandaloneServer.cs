using BaseServer.Commands;
using BaseServer.Core;
using Game;
using Game.DataTypes;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Network.ClientPackets;
using Game.Scheduler;
using Game.Services;
using GameServices;
using LisergyServer.Commands;
using LisergyServer.Core;
using System;

namespace MapServer
{
    /// <summary>
    /// Unified one server for everything, for now
    /// </summary>
    public class StandaloneServer : SocketServer
    {
        private LisergyGame _game;
        private GameScheduler _scheduler;
        private GameServerNetwork _network;
        private AccountService _accountService;
        private BattleService _battleService;
        private WorldService _worldService;
        private CourseService _courseService;
        private ConnectionService _connectionService;

        public override ServerType GetServerType() => ServerType.WORLD;

        public StandaloneServer(LisergyGame game, int port) : base(port)
        {
            Serialization.LoadSerializers();
            _game = game;
            _scheduler = game.Scheduler as GameScheduler;
            _network = game.Network as GameServerNetwork;
            _accountService = new AccountService();
            _battleService = new BattleService(_game);
            _worldService = new WorldService(_game);
            _courseService = new CourseService(_game);
            _connectionService = new ConnectionService();
            _network.OnOutgoingPacket += HandleOutgoingPacket;
        }

        public override void RegisterConsoleCommands(ConsoleCommandExecutor executor)
        {
            executor.RegisterCommand(new TileCommand(_game));
            executor.RegisterCommand(new TaskCommand(_game));
            executor.RegisterCommand(new BattlesCommand(_game, _battleService));
            executor.RegisterCommand(new ServerCommand(_game));
        }

        public override void Tick()
        {
            _scheduler.Tick(DateTime.UtcNow);
        }

        public override void Connect(int connectionID) { }

        public override void Disconnect(int connectionID)
        {
            _accountService.Disconnect(connectionID);
            _connectionService.Disconnect(connectionID);
        }

        /// <summary>
        /// Handles whenever this server receives input from an authenticated connection
        /// </summary>
        public override void ReceiveAuthenticatedPacket(int connectionId, BasePacket input)
        {
            var connectedPlayer = _connectionService.GetConnectedPlayer(connectionId);
            _game.Network.ReceiveInput(connectedPlayer.PlayerId, input);
        }

        /// <summary>
        /// Handles whenever this server wants to send an outgoing packet for a player
        /// </summary>
        private void HandleOutgoingPacket(GameId player, BasePacket packet)
        {
            var connectedPlayer = _connectionService.GetConnectedPlayer(player);
            connectedPlayer?.Send(packet);
        }



        protected override bool IsAuthenticated(int connectionID)
        {
            var connectedAccount = _accountService.GetAuthenticatedConnection(connectionID);
            var hasAuth = connectedAccount != null;
            if (!hasAuth) Log.Error($"Connection {connectionID} is not authenticated");
            return hasAuth;
        }

        protected override bool Authenticate(BasePacket ev, int connectionID)
        {
            Account? connectedAccount;
            if (ev is AuthPacket auth)
            {
                connectedAccount = _accountService.Authenticate(auth);
                Send(connectionID, new AuthResultPacket()
                {
                    PlayerID = connectedAccount == null ? GameId.ZERO : connectedAccount.PlayerId,
                    Success = connectedAccount != null
                });
                if (connectedAccount != null)
                {
                    _connectionService.RegisterConnection(connectedAccount.PlayerId, new ConnectedPlayer(_socketServer)
                    {
                        PlayerId = connectedAccount.PlayerId,
                        ConnectionID = connectionID
                    });
                    Log.Debug($"Connection {connectionID} registered authenticated as {connectedAccount.PlayerId}");
                }
            }
            else
            {
                connectedAccount = _accountService.GetAuthenticatedConnection(connectionID);
            }
            var hasAuth = connectedAccount != null;
            if (!hasAuth) Log.Error($"Connection {connectionID} failed auth to send event {ev}");
            return hasAuth;
        }
    }
}
