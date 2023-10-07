using Game;
using Game.DataTypes;
using Game.Events;
using Game.Network;
using Game.Systems.Player;
using GameServices;

using Telepathy;

namespace LisergyServer.Core
{
    public class ConnectedPlayer : IConnectedPlayer
    {
        private Server _server { get; set; }
        public int ConnectionID { get; set; }
        public GameId PlayerId { get; set; }

        public ConnectedPlayer(Server server)
        {
            this._server = server;
        }

        public void Send<PacketType>(PacketType ev) where PacketType : BasePacket
        {
            Log.Debug($"Sending {ev} to {this}");
            this._server.Send(this.ConnectionID, Serialization.FromPacket<PacketType>(ev));
        }

        public virtual bool Online() => this._server.GetClientAddress(this.ConnectionID) != "";
    }
}
