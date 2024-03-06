using Game.ECS;
using Game.Engine.Events;
using Game.World;
using System;

namespace Game.Engine.ECS
{
    public interface IGameSystem
    {
        void On<EventType>(Action<IEntity, EventType> cb) where EventType : IBaseEvent;
        void CallEvent<EventType>(IEntity entityType, EventType ev) where EventType : IBaseEvent;
    }

    public abstract class GameSystem<ComponentType> : IGameSystem where ComponentType : unmanaged, IComponent
    {
        protected SystemEventBus<ComponentType> EntityEvents = new SystemEventBus<ComponentType>();
        public IGame Game { get; private set; }
        public ISystems Systems => Game.Systems;
        public IGameLogic GameLogic => Game.Logic;
        public IGameWorld World => Game.World;
        public IGamePlayers Players => World.Players;
        public GameSystem(LisergyGame game) { Game = game; }
        internal virtual void OnComponentAdded(IEntity owner, ComponentType component) { }
        public virtual void OnDisabled() { }
        public virtual void RegisterListeners() { }
        internal virtual void OnComponentRemoved(IEntity owner, ComponentType component) { }

        public void On<EventType>(Action<IEntity, EventType> cb) where EventType : IBaseEvent
        {
            EntityEvents.On(cb);
        }

        public void CallEvent<EventType>(IEntity entityType, EventType ev) where EventType : IBaseEvent
        {
            EntityEvents.CallEntityEvent(entityType, ev);
        }
    }

    public class LogicSystem<ComponentType, LogicType> : GameSystem<ComponentType> where ComponentType : unmanaged, IComponent where LogicType : IComponentLogic<ComponentType>
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
            if (!entity.Components.Has<ComponentType>())
            {
                throw new Exception($"Entity {entity} Trying to use logic {typeof(LogicType).Name} without having component {typeof(ComponentType).Name}");
            }
            _logic.Entity = entity;
            return _logic;
        }
    }
}
