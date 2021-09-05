using Game;
using Game.Events;
using Game.Events.Bus;
using LisergyMessageQueue;

namespace BattleService
{
    public class BattleServer
    {
        public EventBus NetworkEvents { get; private set; }

        public BattleServerPacketListener Listener;

        public void ReceiveEvent(byte [] msg) {
            var ev = Serialization.ToEventRaw(msg);
            Log.Info($"Received {ev}");
            NetworkEvents.Call(ev);
        }

        public void StartListening()
        {
            EventMQ.StartListening("battles", ReceiveEvent);
        }

        public BattleServer()
        {
            NetworkEvents = new EventBus();
            Listener = new BattleServerPacketListener();
            Serialization.LoadSerializers();
            NetworkEvents.RegisterListener(Listener);
        }
    }
}
