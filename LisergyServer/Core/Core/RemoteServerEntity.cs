using Game;
using LisergyServer.Core;
using Telepathy;

namespace BattleServer.Core
{
    public class RemoteServerEntity : ServerPlayer
    {

        public RemoteServerEntity(int ConnectionID, Server server): base(server)
        {
            this.ConnectionID = ConnectionID;
        }

        public override void Send<EventType>(EventType ev)
        {
            Game.Log.Debug($"Sending {ev} to {this}");
            this._server.Send(this.ConnectionID, Serialization.FromEvent<EventType>(ev));
        }

        public override bool Online()
        {
            return this._server.GetClientAddress(this.ConnectionID) != "";
        }
    }
}
