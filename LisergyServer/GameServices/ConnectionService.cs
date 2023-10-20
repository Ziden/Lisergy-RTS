using Game;
using Game.DataTypes;
using Game.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServices
{
    public interface IConnectedPlayer
    {
        public GameId PlayerId { get; set; }
        void Send<PacketType>(PacketType ev) where PacketType : BasePacket, new();
        int ConnectionID { get; }
    }

    public class ConnectionService
    {
        private Dictionary<GameId, IConnectedPlayer> _connectedById = new Dictionary<GameId, IConnectedPlayer>();
        private Dictionary<int, IConnectedPlayer> _connectedByConnectionId = new Dictionary<int, IConnectedPlayer>();

        public void RegisterConnection(GameId id, IConnectedPlayer user)
        {
            _connectedById[id] = user;
            _connectedByConnectionId[user.ConnectionID] = user;
            Log.Debug($"Player {id} Registered connection id {user.ConnectionID}");
        }

        public void Disconnect(int connectionId)
        {
            if(_connectedByConnectionId.TryGetValue(connectionId, out var user))
            {
                Log.Debug($"Player {user.PlayerId} disconnected from connection id {connectionId}");
                _connectedById.Remove(user.PlayerId);
                _connectedByConnectionId.Remove(user.ConnectionID);
            }
            Log.Error($"Error disconnecting connection {connectionId} - unknown user");
        }

        public IConnectedPlayer GetConnectedPlayer(int connection) => _connectedByConnectionId[connection];
        public IConnectedPlayer GetConnectedPlayer(GameId id)
        {
            _connectedById.TryGetValue(id, out var connected);
            return connected;
        }

        public void Send(GameId user, BasePacket packet)
        {
            _connectedById.TryGetValue(user, out var connectedUser);
            connectedUser.Send(packet);
        }
    }
}
