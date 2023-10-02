using Game.Events;

namespace Game.ECS
{
    public interface IGameSystem
    {
        void CallEvent<EventType>(IEntity entityType, EventType ev) where EventType : BaseEvent;
    }

    public abstract class GameSystem<ComponentType> : IGameSystem where ComponentType : IComponent 
    {
        public SystemEventBus<ComponentType> SystemEvents = new SystemEventBus<ComponentType>();

        internal virtual void OnComponentAdded(IEntity owner, ComponentType component) { }
        public virtual void OnDisabled() { }
        public virtual void OnEnabled() { }
        internal virtual void OnComponentRemoved(IEntity owner, ComponentType component) { }

        public void CallEvent<EventType>(IEntity entityType, EventType ev) where EventType : BaseEvent
        {
            SystemEvents.CallEntityEvent(entityType, ev);
        }
    }
}
