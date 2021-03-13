using Game.Events;
using LisergyServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerTests
{
    public class TestServerPlayer : ServerPlayer
    {
        public static string TEST_ID = "test_player_id";

        public delegate void ReceiveEventHandler(GameEvent ev);
        public event ReceiveEventHandler OnReceiveEvent;
        public List<GameEvent> ReceivedEvents = new List<GameEvent>();

        public bool IsOnline { get; set; }

        public TestServerPlayer() : base(null, null)
        {

        }

        public override void Send<EventType>(EventType ev)
        {
            ev.Sender = this;
            OnReceiveEvent?.Invoke(ev);
            ReceivedEvents.Add(ev);
        }

        public void SendEventToServer(ClientEvent ev)
        {
            EventEmitter.CallEventFromBytes(this, Serialization.FromEvent(ev));
        }

        public List<T> ReceivedEventsOfType<T>() where T : ServerEvent
        {
            return ReceivedEvents.Where(e => e.GetType().IsAssignableFrom(typeof(T))).Select(e => e as T).ToList();
        }

        public override bool Online()
        {
            return this.IsOnline;
        }

        public override string ToString()
        {
            return $"<TestPlayer id={UserID.ToString()}>";
        }
    }

}
