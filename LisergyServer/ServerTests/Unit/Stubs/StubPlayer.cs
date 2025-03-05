using Game;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Engine.Events.Bus;
using Game.Engine.Network;
using Game.Entities;
using Game.Events.ServerEvents;
using Game.Systems.Course;
using Game.Systems.Movement;
using Game.Systems.Player;
using Game.Tile;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Unit.Stubs;

namespace ServerTests
{
    public class TestServerPlayer : PlayerModel, IEventListener
    {
        public event Action<BasePacket> OnReceivedPacket;

        public List<BasePacket> ReceivedPackets = new List<BasePacket>();
        public List<IBaseEvent> TriggeredEvents = new List<IBaseEvent>();

        public bool IsOnline { get; set; }
        private GameServerNetwork _network;

        public PlayerDataComponent PlayerData => Components.Get<PlayerDataComponent>();
        public PlayerVisibilityComponent VisibilityData => Components.Get<PlayerVisibilityComponent>();
        public IEntity GetParty(int ind) => Parties[ind];
        public List<IEntity> Parties => Game.Entities.GetChildren(EntityId, EntityType.Party).ToList();
        public List<IEntity> Buildings => Game.Entities.GetChildren(EntityId, EntityType.Building).ToList();

        public TestServerPlayer(LisergyGame game) : base(game, null)
        {
            IsOnline = true;
            PlayerEntity = game.Entities.CreateEntity(EntityType.Player);
            PlayerEntity.Save(new PlayerProfileComponent(PlayerEntity.EntityId));
            game.Events.OnEventFired += OnEvent;
            _network = game.Network as GameServerNetwork;
        }

        public void SendTestPacket<EventType>(EventType ev) where EventType : BasePacket, new()
        {
            if (ev is EntityUpdatePacket p && p.Type != EntityType.Tile) // avoid flood
            {
                Game.Log.Debug($"Server Sent Packet {ev.GetType().Name} to Player {this}");
            }
            var reSerialized = Serialization.ToBasePacket(Serialization.FromBasePacket(ev));
            PacketPool.Return(ev);
            OnReceivedPacket?.Invoke(reSerialized);
            ReceivedPackets.Add(reSerialized);
        }

        public void ListenTo<EventType>() where EventType : IBaseEvent
        {
            //Game.Events.On<EventType>(this, ev =>
            // {
            //     TriggeredEvents.Add(ev.ShallowClone());
            // });
        }

        private void OnEvent(IBaseEvent e)
        {
            TriggeredEvents.Add(e.FastShallowClone());
        }

        public void SendMoveRequest(IEntity p, TileModel t, CourseIntent intent)
        {
            var player = t.Game.Players[p.OwnerID];
            var path = t.Chunk.World.FindPath(p.GetTile(), t).Select(pa => new Location(pa.X, pa.Y)).ToList();
            var ev = new MoveEntityCommand() { Path = path, Entity = p.EntityId, Intent = intent };
            ev.Sender = player;
            _network.IncomingPackets.Call(ev);
            ev.Execute(p.Game);
        }

        public List<T> ReceivedPacketsOfType<T>() where T : BasePacket
        {
            return ReceivedPackets.Where(e => e.GetType().IsAssignableFrom(typeof(T))).Select(e => e as T).ToList();
        }

        public List<EntityUpdatePacket> ReceivedEntityUpdates(EntityType type)
        {
            return ReceivedPacketsOfType<EntityUpdatePacket>().Where(p => p.Type == type).ToList();
        }

        public List<T> TriggeredEventsOfType<T>() where T : IBaseEvent
        {
            return TriggeredEvents.Where(e => e.GetType().IsAssignableFrom(typeof(T))).Select(e => (T)e).ToList();
        }

        public override string ToString()
        {
            return $"<TestPlayer id={EntityId.ToString()}>";
        }
    }

}
