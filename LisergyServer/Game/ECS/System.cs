using Game.Events;
using System;

namespace Game.ECS
{
    public interface IGameSystem
    {
        void CallEvent<EventType>(IEntity entityType, EventType ev) where EventType : BaseEvent;
    }

    public abstract class GameSystem<ComponentType> : IGameSystem where ComponentType : IComponent
    {
        public SystemEventBus<ComponentType> EntityEvents = new SystemEventBus<ComponentType>();
        public GameLogic Game { get; private set; }

        public GameSystem(GameLogic game) {
            Game = game;
        }

        internal virtual void OnComponentAdded(IEntity owner, ComponentType component) { }
        public virtual void OnDisabled() { }
        public virtual void OnEnabled() { }
        internal virtual void OnComponentRemoved(IEntity owner, ComponentType component) { }

        public void CallEvent<EventType>(IEntity entityType, EventType ev) where EventType : BaseEvent
        {
            EntityEvents.CallEntityEvent(entityType, ev);
        }
    }

    public class LogicSystem<ComponentType, LogicType> : GameSystem<ComponentType> where ComponentType : IComponent where LogicType : IComponentLogic<ComponentType>
    {
        private LogicType _logic;

        public LogicSystem(GameLogic game) : base(game)
        {
            _logic = Activator.CreateInstance<LogicType>();
        }

        public LogicType GetLogic(IEntity entity)
        {
            _logic.Entity = entity;
            return _logic;
        }
    }
}
