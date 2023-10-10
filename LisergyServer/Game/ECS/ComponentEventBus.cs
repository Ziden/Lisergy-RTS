using Game.Events;
using Game.Events.Bus;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Game.ECS
{
    /// <summary>
    /// Event bus that fires specific events for the specific component. Can wrap events knowing the sender and component instance.
    /// </summary>
    public class SystemEventBus<ComponentType> : IEventListener where ComponentType : unmanaged, IComponent
    {
        internal EventBus<IBaseEvent> _bus = new EventBus<IBaseEvent>();
        private IEntity _currentEntity;

        public delegate void EntityEventCallback<EventType>(IEntity entity, EventType ev) where EventType : IBaseEvent;

        /// <summary>
        /// Registers a new listener for the given system. The system is wrapped in a normal message bus/
        /// </summary>
        public void On<EventType>(EntityEventCallback<EventType> callback) where EventType : IBaseEvent
        {
            void ComponentEventWrapper(EventType ev)
            {
                callback(_currentEntity, ev);
            }
            _bus.Register(this, (Action<EventType>)ComponentEventWrapper);
        }

        public void Clear()
        {
            _bus.Clear();
        }

        public void CallEntityEvent<EventType>(IEntity entity, EventType ev) where EventType : IBaseEvent
        {
            var t = typeof(ComponentType);
            _currentEntity = entity;
            _bus.Call(ev);
        }
    }
}
