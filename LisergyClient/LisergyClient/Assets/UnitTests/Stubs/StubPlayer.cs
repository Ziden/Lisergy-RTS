
using Game;
using Game.Player;
using System.Collections.Concurrent;

namespace Assets.UnitTests.Stubs
{
    public class StubPacket
    {
        public byte[] Data;
        public PlayerEntity Sender;
    }

    public class StubPlayer : PlayerEntity
    {
        public override bool Online()
        {
            return true;
        }

        public override void Send<EventType>(EventType ev)
        {
            Log.Debug($"Sending {ev.GetType().Name} from server to client");
            StubServer.OutputStream.Enqueue(new StubPacket()
            {
                Sender = this,
                Data = Serialization.FromEvent(ev)
            });
        }
    }
}
