using BaseServer.Commands;
using BaseServer.Core;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.Network;
using Game.Network.ClientPackets;
using GameServices;
using LisergyServer.Core;

namespace MapServer
{
    /// <summary>
    /// Basic server with handshacke authentication so players can connect after authenticating to 
    /// account service
    /// </summary>
    public abstract class BaseHandshackedServer : SocketServer
    {
        private CryptographyService _cryptoService;
        protected ConnectionService _validConnections;

        public BaseHandshackedServer() : base()
        {
            Serialization.LoadSerializers();     
            _cryptoService = new CryptographyService(EnvironmentConfig.SECRET_KEY.ToString());  
            _validConnections = new ConnectionService(Log);
        }

        public override void Connect(in int connectionID) 
        {
        }

        public override void RegisterConsoleCommands(ConsoleCommandExecutor executor)
        {

        }

        public override void Disconnect(in int connectionID)
        {
            _validConnections.Disconnect(connectionID);
        }

        /// <summary>
        /// Handles whenever this server receives input from an authenticated connection.
        /// This is only called if the player is already authenticated and should never be called
        /// from an unauthenticated connection
        /// </summary>
        public override void ReceivePacketFromPlayer(in int connectionId, BasePacket input)
        {
            var connectedPlayer = _validConnections.GetAuthenticatedConnection(connectionId);
            Log.Debug($"Processing packet {input.GetType().Name} from player {connectedPlayer.PlayerId}");
            OnReceiveAuthenticatedPacket(connectedPlayer.PlayerId, input);
        }

        /// <summary>
        /// Handles whenever this server wants to send an outgoing packet for a player
        /// </summary>
        protected void SendPacketToPlayer(GameId player, BasePacket packet)
        {
            var connectedPlayer = _validConnections.GetConnectedPlayer(player);
            if (connectedPlayer != null)
            {
                Log.Debug($"Sending packet {packet.GetType().Name} to player {connectedPlayer.PlayerId}");
                connectedPlayer.Send(packet);
            }
            else
            {
                Log.Error($"Could not send packet {packet.GetType().Name} to player {player} because he was not connected");
            }
        }

        /// <summary>
        /// Called after receiving a packet in a authenticated connection
        /// </summary>
        public abstract void OnReceiveAuthenticatedPacket(in GameId player, BasePacket packet);

        /// <summary>
        /// Called whenever a player authenticates via handshake
        /// </summary>
        public abstract void OnAuthenticated(ConnectedPlayer player);

        protected override bool IsAuthenticated(in int connectionID)
        {
            return _validConnections.IsConnectionAuthenticated(connectionID);
        }

        protected override bool Authenticate(BasePacket ev, in int connectionID)
        {
            if (ev is HandshakePacket auth)
            {
                if(_cryptoService.IsTokenValid(auth.PlayerId, auth.Token))
                {
                    var pl = new ConnectedPlayer(auth.PlayerId, connectionID, _socketServer);
                    _validConnections.RegisterAuthenticatedConnection(pl);
                    Log.Debug($"Player {auth.PlayerId} did a handshake and is authenticated on connection {connectionID}");
                    OnAuthenticated(pl);
                    return true;
                } else
                {
                    Log.Debug($"Player {auth.PlayerId} failed handshake");
                    return false;
                }
            } else
            {
                return _validConnections.GetAuthenticatedConnection(connectionID) != null;
            }
        }
    }
}
