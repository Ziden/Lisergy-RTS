using Game.DataTypes;
using Game.Events;
using Game.Events.Bus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Game.ECS
{
    public interface IComponentSet
    {
        List<IComponent> GetSyncedComponents();

        IComponent Get(Type t);

        T Get<T>() where T : IComponent;

        T Add<T>(T c) where T : IComponent;

        T Add<T>() where T : IComponent;

        bool Has<T>() where T : IComponent;

        bool Has(Type t);

        void CallEvent(BaseEvent e);

        void RegisterExternalComponentEvents<ComponentType, EventType>(Action<ComponentType, EventType> cb) where EventType : GameEvent where ComponentType : IComponent;
    }

    /// <summary>
    /// Represents a list of components.
    /// </summary>
    public class ComponentSet<EntityType> : IComponentSet where EntityType : IEntity
    {
        internal Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();
        internal List<IComponent> _networked = new List<IComponent>();

        internal EntityType _owner;

        internal static IDictionary<Type, EntitySharedEventBus<EntityType>> _buses = new DefaultValueDictionary<Type, EntitySharedEventBus<EntityType>>();

        public ComponentSet(EntityType owner)
        {
            _owner = owner;
        }

        public List<IComponent> GetSyncedComponents() => _networked;

        public T Get<T>() where T : IComponent => (T)Get(typeof(T));

        public bool Has<T>() where T : IComponent => _components.ContainsKey(typeof(T));

        public bool Has(Type t) => _components.ContainsKey(t);

        public T Add<T>() where T : IComponent => Has<T>() ? Get<T>() : Add<T>(typeof(T), ComponentCreator.Build<T>());

        public T Add<T>(T c) where T : IComponent => Add<T>(typeof(T), c);

        public void CallEvent(BaseEvent ev) { 
            GetEventBus().Call(_owner, ev);
            StrategyGame.GlobalGameEvents.Call(ev);
        }

        public EntitySharedEventBus<EntityType> GetEventBus() => _buses[typeof(EntityType)];

        public void RegisterComponentEvent<ComponentType, EventType>(Action<EntityType, ComponentType, EventType> cb) where EventType : GameEvent where ComponentType : IComponent
        {
            GetEventBus().RegisterComponentEvent(cb);
        }

        public IComponent Get(Type t)
        {
            if (_components.TryGetValue(t, out var component))
                return component;
            return null;
        }


        public T Add<T>(Type type, IComponent component) where T : IComponent
        {
            _components[type] = component;
            SystemRegistry<T, EntityType>.OnAddComponent(_owner, GetEventBus());
            if (component.GetType().GetCustomAttributes().Any(a => a.GetType() == typeof(SyncedComponent)))
            {
                _networked.Add(component);
            }
            return (T)component;
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            SystemRegistry<T, EntityType>.OnRemovedComponent(_owner, GetEventBus());
        }

        public void ClearListeners()
        {
            foreach (var bus in _buses.Values)
            {
                bus.Clear();
            }
        }

        public void RegisterExternalComponentEvents<ComponentType, EventType>(Action<ComponentType, EventType> cb)

            where ComponentType : IComponent
            where EventType : GameEvent
        {
            if (!cb.GetMethodInfo().IsStatic)
            {
                throw new Exception("External callbacks registered in a shared event bus must be static");
            }

            GetEventBus().RegisterComponentEvent((EntityType e, ComponentType c, EventType ev) => cb(c, ev));
        }
    }
}
