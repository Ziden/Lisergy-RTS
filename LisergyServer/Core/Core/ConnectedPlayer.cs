using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.Network;
using GameServices;
using System;
using System.Runtime.InteropServices;
using Telepathy;

namespace LisergyServer.Core
{
    public class ConnectedPlayer : IConnectedPlayer
    {
        private GameId _playerId;

        private Server _server { get; set; }
        public int ConnectionID { get; set; }
        public ref GameId PlayerId => ref _playerId;

        public ConnectedPlayer(in GameId playerid, in int connectionId, Server server)
        {
            PlayerId = playerid;
            _server = server;
            ConnectionID = connectionId;
        }

        public void Send<PacketType>(PacketType ev) where PacketType : BasePacket, new()
        {
            var bytes = Serialization.FromAnyType(ev);
            PacketPool.Return(ev);
            SendBytes(bytes);
        }

     
        public virtual bool Online() => _server.GetClientAddress(ConnectionID) != "";

        public void SendBytes(in ReadOnlyMemory<byte> data)
        {
            this._server.Send(ConnectionID, data.ToArray());
        }
    }
}
