using Game.Events;
using Game.Systems.Player;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Game.ECS
{
    public unsafe interface IComponentSet
    {
        IReadOnlyList<IComponent> GetSyncedComponents(PlayerEntity receiver);
        IReadOnlyDictionary<Type, IComponent> All();
        IComponent Get(Type t);
        T Get<T>() where T : IComponent;
        void Save<T>(in T c) where T : IComponent;
        T Add<T>(in T c) where T : IComponent;
        bool Has<T>() where T : IComponent;
        void CallEvent(BaseEvent e);
    }

    /// <summary>
    /// Represents a list of components.
    /// </summary>
    public unsafe class ComponentSet : IComponentSet
    {
        internal Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();
        internal List<Type> _networkedPublic = new List<Type>();
        internal List<Type> _networkedSelf = new List<Type>();

        internal IEntity _entity;
        internal PlayerEntity _owner;

        private List<IComponent> _returnBuffer = new List<IComponent>();

        public ComponentSet(IEntity entity, PlayerEntity owner = null)
        {
            _entity = entity;
            _owner = owner;
        }

        public IReadOnlyList<IComponent> GetSyncedComponents(PlayerEntity receiver)
        {
            _returnBuffer.Clear();
            if(receiver == _owner)
            {
                foreach (var t in _networkedSelf) _returnBuffer.Add(_components[t]);
            } else
            {
                foreach (var t in _networkedPublic) _returnBuffer.Add(_components[t]);
            }
            return _returnBuffer;
        }

        public IReadOnlyDictionary<Type, IComponent> All() => _components;

        public T Get<T>() where T : IComponent
        {
            return (T)Get(typeof(T));
        }

        public bool Has<T>() where T : IComponent
        {
            return _components.ContainsKey(typeof(T));
        }

        public T Add<T>() where T : IComponent
        {
            return Has<T>() ? Get<T>() : Add<T>(typeof(T), ComponentCreator.Build<T>());
        }

        public T Add<T>(in T c) where T : IComponent
        {
            return Add<T>(c.GetType(), c);
        }

        public void CallEvent(BaseEvent ev)
        {
            _entity.Game.Systems.CallEvent(_entity, ev);
        }

        public IComponent Get(Type t)
        {
            return _components.TryGetValue(t, out IComponent component) ? component : null;
        }

        public T Add<T>(Type type, in T component) where T : IComponent
        {
            _components[type] = component;
            var sync = type.GetCustomAttribute(typeof(SyncedComponent)) as SyncedComponent;
            if (sync != null)
            {
                _networkedSelf.Add(component.GetType());
                if(!sync.OnlyMine)
                    _networkedPublic.Add(component.GetType());
            }
            return component;
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            throw new NotImplementedException();
        }

        public void Save<T>(in T c) where T : IComponent
        {
            _components[c.GetType()] = c;
        }
    }
}
