using Game;
using Game.Entity;
using Game.Events;
using LisergyServer.Core;
using System.Collections.Generic;
using System.Linq;

namespace ServerTests
{
    public class TestServerPlayer : ServerPlayer
    {

        public delegate void ReceiveEventHandler(BaseEvent ev);
        public event ReceiveEventHandler OnReceiveEvent;
        public List<BaseEvent> ReceivedEvents = new List<BaseEvent>();

        public bool IsOnline { get; set; }

        public TestServerPlayer() : base(null)
        {

        }

        public override void Send<EventType>(EventType ev)
        {
            ev.Sender = this;
            OnReceiveEvent?.Invoke(ev);
            ReceivedEvents.Add(ev);
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