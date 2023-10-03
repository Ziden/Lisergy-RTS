using Game.DataTypes;
using Game.Events;
using Game.Systems.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Game.ECS
{
    public interface IComponentSet
    {
        List<IComponent> GetSyncedComponents(PlayerEntity receiver);

        IComponent Get(Type t);

        T Get<T>() where T : IComponent;

        T Add<T>(T c) where T : IComponent;

        T Add<T>() where T : IComponent;

        bool Has<T>() where T : IComponent;

        bool Has(Type t);

        void CallEvent(BaseEvent e);

    }

    /// <summary>
    /// Represents a list of components.
    /// </summary>
    public class ComponentSet : IComponentSet
    {
        internal Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();
        internal List<IComponent> _networkedPublic = new List<IComponent>();
        internal List<IComponent> _networkedSelf = new List<IComponent>();

        internal IEntity _entity;
        internal PlayerEntity _owner;

        public ComponentSet(IEntity entity, PlayerEntity owner = null)
        {
            _entity = entity;
            _owner = owner;
        }

        public List<IComponent> GetSyncedComponents(PlayerEntity receiver)
        {
            return receiver != null && receiver == _owner ? _networkedSelf : _networkedPublic;
        }

        public T Get<T>() where T : IComponent
        {
            return (T)Get(typeof(T));
        }

        public bool Has<T>() where T : IComponent
        {
            return _components.ContainsKey(typeof(T));
        }

        public bool Has(Type t)
        {
            return _components.ContainsKey(t);
        }

        public T Add<T>() where T : IComponent
        {
            return Has<T>() ? Get<T>() : Add<T>(typeof(T), ComponentCreator.Build<T>());
        }

        public T Add<T>(T c) where T : IComponent
        {
            return Add<T>(c.GetType(), c);
        }

        public void CallEvent(BaseEvent ev)
        {
            GameSystems.HACK_REMOVE_ME.Game.Events.Call(ev);
            foreach (var kp in _components)
            {
                GameSystems.HACK_REMOVE_ME.CallEvent(kp.Key, _entity, ev);
            } 
        }

        public IComponent Get(Type t)
        {
            return _components.TryGetValue(t, out IComponent component) ? component : null;
        }

        public T Add<T>(Type type, T component) where T : IComponent
        {
            _components[type] = component;
            var sync = type.GetCustomAttribute(typeof(SyncedComponent)) as SyncedComponent;
            if (sync != null)
            {
                _networkedSelf.Add(component);
                if(!sync.OnlyMine)
                    _networkedPublic.Add(component);
            }
            return component;
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            throw new NotImplementedException();
        }
    }
}
