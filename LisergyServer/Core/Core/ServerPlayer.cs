using Game;
using Game.Events;
using Game.Player;
using System.Collections.Generic;
using Telepathy;

namespace LisergyServer.Core
{
    public class ServerPlayer : PlayerEntity
    {
        protected Server _server { get; set; }
        public int ConnectionID { get; set; }

        public ServerPlayer(Server server)
        {
            this._server = server;
        }

        public HashSet<BaseEvent> _sent = new HashSet<BaseEvent>();

        public override void Send<EventType>(EventType ev)
        {
            Log.Debug($"Sending {ev} to {this}");
            this._server.Send(this.ConnectionID, Serialization.FromEvent<EventType>(ev));
        }

        public override bool Online()
        {
            return this._server.GetClientAddress(this.ConnectionID) != "";
        }
    }
}
