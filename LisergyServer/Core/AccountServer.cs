using BaseServer.Commands;
using BaseServer.Core;
using Game.Engine;
using Game.Engine.Network;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using Game.Network.ServerPackets;
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

        public override void Connect(in int connectionID) { }

        public override void Disconnect(in int connectionID)
        {
            _accountService.Disconnect(connectionID);
            _connectionService.Disconnect(connectionID);
        }

        public override void ReceivePacketFromPlayer(in int connectionId, BasePacket input) { }

        protected override bool IsAuthenticated(in int connectionID)
        {
            var connectedAccount = _accountService.GetAuthenticatedConnection(connectionID);
            var hasAuth = connectedAccount != null;
            return hasAuth;
        }

        protected override bool Authenticate(BasePacket ev, in int connectionID)
        {
            Account? connectedAccount;
            if (ev is LoginPacket auth)
            {
                connectedAccount = _accountService.Authenticate(auth);
                var authResult = new LoginResultPacket()
                {
                    Success = connectedAccount != null,
                };
                if (authResult.Success)
                {
                    authResult.Token = _cryptographyService.GenerateToken(connectedAccount.Profile.PlayerId);
                    authResult.TokenDuration = _cryptographyService.TokenDuration;
                    authResult.Profile = connectedAccount.Profile;
                }
                Send(connectionID, authResult);
                if (connectedAccount != null)
                {
                    _connectionService.RegisterAuthenticatedConnection(new ConnectedPlayer(connectedAccount.Profile.PlayerId, connectionID, _socketServer));
                    Log.Debug($"Connection {connectionID} registered authenticated as {connectedAccount.Profile}");
                }
                return true;
            }
            Log.Error($"Connection {connectionID} tried to authenticate with {ev.GetType().Name}");
            Send(connectionID, new InvalidSessionPacket());
            return false;
        }
    }
}
