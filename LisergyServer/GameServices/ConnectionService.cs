using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.Network;
using System.Collections.Generic;

namespace GameServices
{
    public interface IConnectedPlayer
    {
        public ref GameId PlayerId { get; }
        void Send<PacketType>(PacketType ev) where PacketType : BasePacket, new();
        void Send(in byte[] data);
        int ConnectionID { get; }
    }

    public class ConnectionService
    {
        private Dictionary<GameId, IConnectedPlayer> _connectedById = new Dictionary<GameId, IConnectedPlayer>();
        private Dictionary<int, IConnectedPlayer> _connectedByConnectionId = new Dictionary<int, IConnectedPlayer>();
        private IGameLog _log;

        public ConnectionService(IGameLog log)
        {
            _log = log;
        }

        public void Broadcast(BasePacket packet)
        {
            var bytes = Serialization.FromBasePacket(packet);
            foreach (var u in _connectedById.Values)
            {
                _log.Debug($"Broadcasting {packet.GetType()} to player {u.PlayerId}");
                u.Send(bytes);
            }
        }

        public void RegisterAuthenticatedConnection(IConnectedPlayer user)
        {
            _connectedById[user.PlayerId] = user;
            _connectedByConnectionId[user.ConnectionID] = user;
            _log.Debug($"Player {user.PlayerId} Registered connection id {user.ConnectionID}");
        }

        public void Disconnect(in int connectionId)
        {
            if (_connectedByConnectionId.TryGetValue(connectionId, out var user))
            {
                _log.Debug($"Player {user.PlayerId} disconnected from connection id {connectionId}");
                _connectedById.Remove(user.PlayerId);
                _connectedByConnectionId.Remove(user.ConnectionID);
            }
            _log.Error($"Error disconnecting connection {connectionId} - unknown user");
        }

        public bool IsConnectionAuthenticated(in int connection)
        {
            return _connectedByConnectionId.ContainsKey(connection);
        }

        public IConnectedPlayer GetAuthenticatedConnection(in int connection) => _connectedByConnectionId[connection];
        public IConnectedPlayer GetConnectedPlayer(in GameId id)
        {
            _connectedById.TryGetValue(id, out var connected);
            return connected;
        }
    }
}
