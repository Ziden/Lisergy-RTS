using Game.Events;
using Game.Events.Bus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Game.ECS
{
    public class ComponentSet<EntityType> where EntityType : IEntity
    {
        internal Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

        internal EntityType _owner;

        internal static ConcurrentDictionary<Type, EntitySharedEventBus<EntityType>> _buses = new ConcurrentDictionary<Type, EntitySharedEventBus<EntityType>>();

        public EntitySharedEventBus<EntityType> GetEventBus()
        {
            if (!_buses.TryGetValue(typeof(EntityType), out var bus))
            {
                bus = new EntitySharedEventBus<EntityType>();
                _buses[typeof(EntityType)] = bus;
            }
            return bus;
        }

        public ComponentSet(EntityType owner)
        {
            _owner = owner;
        }

        public void CallEvent(BaseEvent ev)
        {
            GetEventBus().Call(_owner, ev);
        }

        public void RegisterComponentEvent<EventType, ComponentType>(Action<EntityType, ComponentType, EventType> cb) where EventType: GameEvent where ComponentType : IComponent 
        {
            GetEventBus().RegisterComponentEvent(cb);
        }

        public T Get<T>() where T : IComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                return (T)component;
            }
            return default;
        }

        public T AddComponent<T>() where T : IComponent
        {
            var component = ComponentCreator.Build<T>();
            _components[typeof(T)] = component;
            SystemRegistry<T, EntityType>.OnAddComponent(_owner, GetEventBus());
            return component;
        }

        public T AddComponent<T>(T component) where T : IComponent
        {
            _components[typeof(T)] = component;
            SystemRegistry<T, EntityType>.OnAddComponent(_owner, GetEventBus());
            return component;
        }

        public void RemoveComponent<T>()
        {
            // TODO: Remove & clear listener
        }
    }

    public interface IComponent
    {

    }
}
