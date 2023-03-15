using Game.Events;
using Game.Events.Bus;
using System;
using System.Collections.Generic;

namespace Game.ECS
{

    public class ComponentSet<EntityType> where EntityType : IEntity
    {
        private Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

        private EntityType _owner;

        private ComponentEventBus<EntityType> _events = new ComponentEventBus<EntityType>();

        public ComponentSet(EntityType owner)
        {
            _owner = owner;
        }

        public void CallEvent(BaseEvent ev)
        {
            _events.Call(_owner, ev);
        }

        public T GetComponent<T>() where T : class, IComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                return component as T;
            }
            return default;
        }

        public void AddComponent<T>() where T : class, IComponent
        {
            var component = Activator.CreateInstance<T>();
            _components[typeof(T)] = component;
            SystemRegistry<T, EntityType>.OnAddComponent(_owner, component, _events);
        }
    }

    public interface IComponent
    {
   
    }
}
