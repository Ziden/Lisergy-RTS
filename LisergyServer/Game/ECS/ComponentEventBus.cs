using Game.Events;
using Game.Events.Bus;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Game.ECS
{
    /// <summary>
    /// Event bus that fires specific events for the specific component. Can wrap events knowing the sender and component instance.
    /// </summary>
    public class ComponentEventBus<EntityType> : IEventListener where EntityType : IEntity
    {
        internal EventBus<BaseEvent> _bus = new EventBus<BaseEvent>();
        internal HashSet<string> _registered = new HashSet<string>();

        private bool AlreadyRegistered<EventType, ComponentType>()
        {
            var key = $"{typeof(EventType).Name}/{typeof(ComponentType).Name}";
            if (_registered.Contains(key)) return true;
            _registered.Add(key);
            Console.WriteLine("Registed " + key);
            return false;
        }

        private EntityType _currentEntity;

        public EntityType GetCurrentEntity() => _currentEntity;

        /// <summary>
        /// Registers events that can be targeted to the entity. The components can pickup those events.
        /// We only subscribe once per ComponentType/EventType combination so we can swap entities that are receiving the events.
        /// </summary>
        public void RegisterComponentEvent<EventType, ComponentType>(
            IGameSystem system, 
            Action<EntityType, ComponentType, EventType> callback)
            where EventType : BaseEvent where ComponentType : IComponent 
        {
            if(AlreadyRegistered<EventType, ComponentType>())
            {
                return;
            }
            Action<EventType> registeredCallback = ev =>
            {
                var entity = GetCurrentEntity();
                var component = entity.GetComponent<ComponentType>();
                callback(entity, component, ev);
            };
            _bus.Register<EventType>(system, registeredCallback);
        }

        public void RegisterEvent<EventType>(EventType ev)
        {

        }

        public void Call<EventType>(EntityType entity, EventType ev) where EventType : BaseEvent
        {
            _currentEntity = entity;
            _bus.Call(ev);
        }
    }
}
