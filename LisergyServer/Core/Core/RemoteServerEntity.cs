using Game;
using LisergyServer.Core;
using Telepathy;

namespace BaseServer.Core
{
    public class RemoteServerEntity : ServerPlayer
    {

        public RemoteServerEntity(int ConnectionID, Server server) : base(server)
        {
            this.ConnectionID = ConnectionID;
        }

        public override void Send<EventType>(EventType ev)
        {
            Log.Debug($"Sending {ev} to {this}");
            _ = _server.Send(ConnectionID, Serialization.FromEvent(ev));
        }

        public override bool Online()
        {
            return _server.GetClientAddress(ConnectionID) != "";
        }
    }
}
