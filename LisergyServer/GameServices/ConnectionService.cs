using Game.DataTypes;
using Game.Network;
using System.Collections.Generic;

namespace GameServices
{

    public interface IConnectedPlayer
    {
        public GameId PlayerId { get; set; }
        void Send<PacketType>(PacketType ev) where PacketType : BasePacket;
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
        }

        public void Disconnect(int connectionId)
        {
            if(_connectedByConnectionId.TryGetValue(connectionId, out var user))
                _connectedById.Remove(user.PlayerId);
            _connectedByConnectionId.Remove(user.ConnectionID); 
        }

        public IConnectedPlayer GetConnectedPlayer(int connection) => _connectedByConnectionId[connection];
        public IConnectedPlayer GetConnectedPlayer(GameId id) => _connectedById[id];

        public void Send(GameId user, BasePacket packet)
        {
            _connectedById.TryGetValue(user, out var connectedUser);
            connectedUser.Send(packet);
        }
    }
}
