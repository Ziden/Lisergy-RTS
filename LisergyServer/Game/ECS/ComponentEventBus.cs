using Game.Events;
using Game.Events.Bus;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Game.ECS
{

    /// <summary>
    /// To avoid boxing/unboxing and gain speed
    /// </summary>
    public class EntityContext<EntityType> where EntityType : IEntity
    {
        public EntityType Entity;

        internal static EntityContext<EntityType> Context;
    }

    /// <summary>
    /// Event bus that fires specific events for the specific component. Can wrap events knowing the sender and component instance.
    /// </summary>
    public class EntityEventBus : IEventListener
    {
        internal EventBus<BaseEvent> _bus = new EventBus<BaseEvent>();
        internal HashSet<string> _registered = new HashSet<string>();

        public virtual ComponentType GetComponent<ComponentType>(IEntity e) where ComponentType : IComponent
        {
            return e.Components.Get<ComponentType>();
        }

        public bool AlreadyRegistered<EventType, ComponentType>()
        {
            var key = $"{typeof(EventType).Name}/{typeof(ComponentType).Name}";
            if (_registered.Contains(key)) return true;
            _registered.Add(key);
            return false;
        }

        private IEntity _currentEntity;

        public IEntity GetCurrentEntity() => _currentEntity;

        /// <summary>
        /// Registers events that can be targeted to the entity. The components can pickup those events.
        /// We only subscribe once per ComponentType/EventType combination so we can swap entities that are receiving the events.
        /// </summary>
        public void RegisterComponentEvent<EntityType, EventType, ComponentType>(
            Action<EntityType, ComponentType, EventType> callback)
            where EventType : BaseEvent where ComponentType : IComponent where EntityType : IEntity
        {
            if (AlreadyRegistered<EventType, ComponentType>())
            {
                return;
            }
            Action<EventType> componentEventCallback = ev =>
            {
                var entity = GetCurrentEntity();
                var component = GetComponent<ComponentType>(entity);
                // TODO: Remove unboxing below for speed
                if (component != null) callback((EntityType)entity, component, ev);
            };
            _bus.Register(this, componentEventCallback);
        }

        public void Clear()
        {
            _bus.Clear();
        }

        public void RegisterEvent<EventType>(EventType ev)
        {

        }

        public void Call<EventType>(IEntity entity, EventType ev) where EventType : BaseEvent
        {
            _currentEntity = entity;
            _bus.Call(ev);
        }
    }
}
