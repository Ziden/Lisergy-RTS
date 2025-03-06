using Game.Engine.Events;
using Game.Engine.Events.Bus;
using Game.World;
using System;

namespace Game.Engine.ECLS
{
    public interface IGameSystem
    {
        void OnEntityEvent<EventType>(Action<IEntity, EventType> cb) where EventType : IBaseEvent;
        void CallEvent<EventType>(IEntity entityType, EventType ev) where EventType : IBaseEvent;
    }

    public abstract class GameSystem<ComponentType> : IGameSystem, IEventListener where ComponentType : IComponent
    {
        protected SystemEventBus<ComponentType> EntityEvents = new SystemEventBus<ComponentType>();
        public IGame Game { get; private set; }
        public IGameLogic GameLogic => Game.Logic;
        public IGameWorld World => Game.World;
        public IGamePlayers Players => World.Players;
        public GameSystem(LisergyGame game) { Game = game; }
        internal virtual void OnComponentAdded(IEntity owner, ComponentType component) { }
        public virtual void OnDisabled() { }
        public virtual void RegisterListeners() { }
        internal virtual void OnComponentRemoved(IEntity owner, ComponentType component) { }

        /// <summary>
        /// Fired whenever an entity receives an event that matches the component defined in the system component type
        /// </summary>
        public void OnEntityEvent<EventType>(Action<IEntity, EventType> cb) where EventType : IBaseEvent
        {
            EntityEvents.On(cb);
        }

        // TODO: Separate better entity and global events
        /// <summary>
        /// Fired whenever any entity receive an event of the given type
        /// </summary>
        public void OnAnyEvent<EventType>(Action<EventType> cb) where EventType : IBaseEvent
        {
            Game.Events.On(this, cb);
        }

        public void CallEvent<EventType>(IEntity entityType, EventType ev) where EventType : IBaseEvent
        {
            EntityEvents.CallEntityEvent(entityType, ev);
        }

        public EntityLogic GetLogic(IEntity e) => GameLogic.GetEntityLogic(e);
    }

    public class LogicSystem<ComponentType, LogicType> : GameSystem<ComponentType> where ComponentType : IComponent where LogicType : BaseEntityLogic<ComponentType>
    {
        private LogicType _logic;

        public LogicSystem(LisergyGame game) : base(game)
        {
            _logic = Activator.CreateInstance<LogicType>();
        }

        /// <summary>
        /// Gets given logic for given entity
        /// </summary>
        public LogicType GetLogic(IEntity entity)
        {
            _logic.CurrentEntity = entity;
            return _logic;
        }
    }
}
