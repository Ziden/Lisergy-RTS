using BaseServer.Commands;
using BaseServer.Core;
using Game;
using Game.DataTypes;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Network.ClientPackets;
using GameServices;
using LisergyServer.Core;

namespace MapServer
{
    /// <summary>
    /// Just responsible for authenticating user
    /// </summary>
    public class AccountServer : SocketServer
    {
        public override ServerType GetServerType() => ServerType.ACCOUNT;

        private AccountService _accountService;
        private ConnectionService _connectionService;
        private CryptographyService _cryptographyService;

        public AccountServer() : base()
        {
            Serialization.LoadSerializers();
            _accountService = new AccountService(Log);
            _cryptographyService = new CryptographyService(EnvironmentConfig.SECRET_KEY.ToString());
            _connectionService = new ConnectionService(Log);
        }

        public override void RegisterConsoleCommands(ConsoleCommandExecutor executor) { }

        public override void Tick()
        {
           
        }

        public override void Connect(int connectionID) { }

        public override void Disconnect(int connectionID)
        {
            _accountService.Disconnect(connectionID);
            _connectionService.Disconnect(connectionID);
        }

        public override void ReceiveAuthenticatedPacket(int connectionId, BasePacket input) { }

        protected override bool IsAuthenticated(int connectionID)
        {
            var connectedAccount = _accountService.GetAuthenticatedConnection(connectionID);
            var hasAuth = connectedAccount != null;
            return hasAuth;
        }

        protected override bool Authenticate(BasePacket ev, int connectionID)
        {
            Account? connectedAccount;
            if (ev is LoginPacket auth)
            {
                connectedAccount = _accountService.Authenticate(auth);
                Send(connectionID, new LoginResultPacket()
                {
                    PlayerID = connectedAccount == null ? GameId.ZERO : connectedAccount.PlayerId,
                    Success = connectedAccount != null,
                    Token = _cryptographyService.GenerateToken(connectedAccount.PlayerId)
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
            return false;
        }
    }
}
