using Game.Events;
using Game.Events.Bus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Game.ECS
{
    /// <summary>
    /// Event bus that fires specific events for the specific component. Can wrap events knowing the sender and component instance.
    /// </summary>
    public class SystemEventBus<ComponentType> : IEventListener where ComponentType : IComponent
    {
        internal EventBus<BaseEvent> _bus = new EventBus<BaseEvent>();
        private IEntity _currentEntity;

        /// <summary>
        /// Registers a new listener for the given system. The system is wrapped in a normal message bus/
        /// </summary>
        public void On<EventType>(Action<IEntity, ComponentType, EventType> callback) where EventType : BaseEvent
        {
            void ComponentEventWrapper(EventType ev)
            {
                var component = _currentEntity.Components.Get<ComponentType>();
                if (component == null) return;
                callback(_currentEntity, component, ev);
            }

            _bus.Register(this, (Action<EventType>)ComponentEventWrapper);
        }

        public void Clear()
        {
            _bus.Clear();
        }

        public void CallEntityEvent<EventType>(IEntity entity, EventType ev) where EventType : BaseEvent
        {
            _currentEntity = entity;
            _bus.Call(ev);
        }
    }
}
