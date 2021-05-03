using Game;
using Game.Entity;
using Game.Events;
using Game.Movement;
using Game.World;
using LisergyServer.Core;
using System.Collections.Generic;
using System.Linq;

namespace ServerTests
{
    public class TestServerPlayer : ServerPlayer
    {
        public static string TEST_ID = "test_player_id";

        public delegate void ReceiveEventHandler(GameEvent ev);
        public event ReceiveEventHandler OnReceiveEvent;
        public List<GameEvent> ReceivedEvents = new List<GameEvent>();

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

        public void SendMoveRequest(Party p, Tile t, MovementIntent intent)
        {
            var path = t.Chunk.ChunkMap.FindPath(p.Tile, t).Select(pa => new Position(pa.X, pa.Y)).ToList();
            var ev = new MoveRequestEvent() { Path = path, PartyIndex = p.PartyIndex, Intent = intent };
            ev.Sender = this;
            SendEventToServer(ev);
        }

        public void SendEventToServer<Ev>(Ev ev) where Ev: ClientEvent
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
