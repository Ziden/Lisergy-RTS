using Game;
using Game.DataTypes;
using Game.Entity.Entities;
using Game.Events;
using Game.Events.Bus;
using Game.Events.GameEvents;
using Game.Movement;
using Game.World;
using LisergyServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerTests
{
    public class TestServerPlayer : ServerPlayer, IEventListener
    {
        public static GameId TEST_ID = GameId.Generate();

        public delegate void ReceiveEventHandler(BaseEvent ev);
        public event ReceiveEventHandler OnReceiveEvent;
        public List<BaseEvent> ReceivedEvents = new List<BaseEvent>();

        public bool IsOnline { get; set; }

        public TestServerPlayer() : base(null)
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
            StrategyGame.GlobalGameEvents.Register<EventType>(this, ev =>
            {
                ReceivedEvents.Add(ev);
            });
        }

        public void SendMoveRequest(PartyEntity p, Tile t, MovementIntent intent)
        {
            var path = t.Chunk.Map.FindPath(p.Tile, t).Select(pa => new Position(pa.X, pa.Y)).ToList();
            var ev = new MoveRequestPacket() { Path = path, PartyIndex = p.PartyIndex, Intent = intent };
            ev.Sender = this;
            t.Chunk.Map.World.Game.NetworkEvents.Call(ev);
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
