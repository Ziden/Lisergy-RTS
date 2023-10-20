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
    public class ConnectionHubServer : SocketServer
    {
        private ConnectionHubNetwork _network;
        private AccountService _accountService;
        private ConnectionService _connectionService;

        public override ServerType GetServerType() => ServerType.HUB;

        public ConnectionHubServer(int port) : base(port)
        {
            Serialization.LoadSerializers();
            _network = new ConnectionHubNetwork();
            _accountService = new AccountService();
            _connectionService = new ConnectionService();
            _network.OnOutgoingPacket += HandleOutgoingPacket;
        }

        public override void Tick()
        {
           
        }

        public override void Connect(int connectionID) { }

        public override void Disconnect(int connectionID)
        {
            _accountService.Disconnect(connectionID);
            _connectionService.Disconnect(connectionID);
        }

        public override void ReceiveAuthenticatedPacket(int connectionId, BasePacket input)
        {
            var connectedPlayer = _connectionService.GetConnectedPlayer(connectionId);
           // _network.SendToServer(ServerType.WORLD)

        }

        private void HandleOutgoingPacket(ServerType server, RoutedPacket packet)
        {
            //var connectedPlayer = _connectionService.GetConnectedPlayer(player);
            //connectedPlayer?.Send(packet);
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

        public override void RegisterConsoleCommands(ConsoleCommandExecutor executor)
        {
            throw new NotImplementedException();
        }
    }
}
