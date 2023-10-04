using Game;
using Game.Events;
using Game.Events.Bus;
using Game.Network.ClientPackets;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Systems.World;
using Game.Tile;
using LisergyServer.Core;
using System.Collections.Generic;
using System.Linq;

namespace ServerTests
{
    public class TestServerPlayer : ServerPlayer, IEventListener
    {
        public delegate void ReceiveEventHandler(BaseEvent ev);
        public event ReceiveEventHandler OnReceiveEvent;
        public List<BaseEvent> ReceivedEvents = new List<BaseEvent>();

        public bool IsOnline { get; set; }

        public TestServerPlayer(GameLogic game = null) : base(null, game)
        {
            IsOnline = true;
        }

        public override void Send<EventType>(EventType ev)
        {
            ev.Sender = this;
            var copy = Serialization.FromEventRaw(ev);
            var reSerialized = Serialization.ToEventRaw(copy);
            OnReceiveEvent?.Invoke(reSerialized);
            ReceivedEvents.Add(reSerialized);
        }

        public void ListenTo<EventType>() where EventType : GameEvent
        {
            Game.Events.Register<EventType>(this, ev =>
            {
                ReceivedEvents.Add(ev);
            });
        }

        public void SendMoveRequest(PartyEntity p, TileEntity t, MovementIntent intent)
        {
            var path = t.Chunk.Map.FindPath(p.Tile, t).Select(pa => new MapPosition(pa.X, pa.Y)).ToList();
            var ev = new MoveRequestPacket() { Path = path, PartyIndex = p.PartyIndex, Intent = intent };
            ev.Sender = this;
            Game.NetworkPackets.Call(ev);
        }

        public List<T> ReceivedEventsOfType<T>() where T : BaseEvent
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
