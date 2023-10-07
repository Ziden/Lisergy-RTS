using Game;
using Game.Events;
using Game.Events.Bus;
using Game.Network;
using Game.Network.ClientPackets;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Systems.Player;
using Game.Tile;
using Game.World;
using LisergyServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerTests
{
    public class TestServerPlayer : PlayerEntity, IEventListener
    {
        public event Action<BasePacket> OnReceivedPacket;

        public List<BasePacket> ReceivedPackets = new List<BasePacket>();
        public List<BaseEvent> TriggeredEvents = new List<BaseEvent>();

        public bool IsOnline { get; set; }
        private GameNetwork _network;

        public TestServerPlayer(LisergyGame game) : base(game)
        {
            IsOnline = true;
            _network = game.Network as GameNetwork;
        }

        public void SendTestPacket<EventType>(EventType ev) where EventType : BasePacket
        {
            var copy = Serialization.FromPacketRaw(ev);
            var reSerialized = Serialization.ToPacketRaw(copy);
            OnReceivedPacket?.Invoke(reSerialized);
            ReceivedPackets.Add(reSerialized);
        }

        public void ListenTo<EventType>() where EventType : BaseEvent
        {
            Game.Events.Register<EventType>(this, ev =>
            {
                TriggeredEvents.Add(ev);
            });
        }

        public void SendMoveRequest(PartyEntity p, TileEntity t, MovementIntent intent)
        {
            var path = t.Chunk.Map.FindPath(p.Tile, t).Select(pa => new Position(pa.X, pa.Y)).ToList();
            var ev = new MoveRequestPacket() { Path = path, PartyIndex = p.PartyIndex, Intent = intent };
            ev.Sender = this;
            _network.IncomingPackets.Call(ev);
        }

        public List<T> ReceivedPacketsOfType<T>() where T : BasePacket
        {
            return ReceivedPackets.Where(e => e.GetType().IsAssignableFrom(typeof(T))).Select(e => e as T).ToList();
        }

        public List<T> TriggeredEventsOfType<T>() where T : BaseEvent
        {
            return TriggeredEvents.Where(e => e.GetType().IsAssignableFrom(typeof(T))).Select(e => e as T).ToList();
        }

        public override string ToString()
        {
            return $"<TestPlayer id={EntityId.ToString()}>";
        }
    }

}
