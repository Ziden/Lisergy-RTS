using Game;
using Game.Events;
using LisergyServer.Auth;
using Telepathy;

namespace LisergyServer.Core
{
    public class ServerPlayer : PlayerEntity
    {
        private Server _server { get; set; }
        public Account Account { get; private set; }
        public int ConnectionID { get; set; }

        public ServerPlayer(Account acc, Server server)
        {
            this.Account = acc;
            this._server = server;
        }

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
