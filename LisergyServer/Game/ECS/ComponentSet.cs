using Game.Events;
using Game.Events.Bus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Game.ECS
{
    
    public class ComponentSet<EntityType> where EntityType : IEntity
    {
        internal Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

        internal EntityType _owner;

        internal static ConcurrentDictionary<Type, ComponentEventBus<EntityType>> _buses = new ConcurrentDictionary<Type, ComponentEventBus<EntityType>>();

        public ComponentEventBus<EntityType> GetEventBus()
        {
            if (!_buses.TryGetValue(typeof(EntityType), out var bus))
            {
                bus = new ComponentEventBus<EntityType>();
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

        public T GetComponent<T>() where T : IComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                return (T)component;
            }
            return default;
        }

        public void AddComponent<T>() where T : IComponent
        {
            var component = Activator.CreateInstance<T>();
            _components[typeof(T)] = component;
            SystemRegistry<T, EntityType>.OnAddComponent(_owner, GetEventBus());
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
