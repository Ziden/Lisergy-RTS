using Game.Events;
using Game.Systems.Player;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Game.ECS
{
    public class ComponentSet : IComponentSet
    {
        internal Dictionary<Type, IComponent> _referenceComponents = new Dictionary<Type, IComponent>();
        internal ComponentPointers _pointerComponents = new ComponentPointers();

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

        /// <summary>
        /// TODO: Use buffers for performance
        /// </summary>
        public IReadOnlyList<IComponent> GetSyncedComponents(PlayerEntity receiver)
        {
            Log.Debug($"Sync component for entity {_entity} of {_owner}");
            _returnBuffer.Clear();
            if (receiver == _owner)
            {
                foreach (var t in _networkedSelf) _returnBuffer.Add(_pointerComponents.AsInterface(t));
            } else
            {
                foreach (var t in _networkedPublic) _returnBuffer.Add(_pointerComponents.AsInterface(t));
            }
            return _returnBuffer;
        }

        public IReadOnlyCollection<Type> All() => _pointerComponents.Keys;
        public ref T Get<T>() where T : unmanaged, IComponent => ref _pointerComponents.AsReference<T>();
        public bool Has<T>() where T : unmanaged, IComponent => _pointerComponents.ContainsKey(typeof(T));
        public bool HasReference<T>() where T : class, IComponent => _referenceComponents.ContainsKey(typeof(T));
        public void Add<T>() where T : unmanaged, IComponent
        {
            _pointerComponents.Alloc<T>();
            TrackSync<T>();
        }

        public T AddReference<T>(in T c) where T : class, IReferenceComponent
        {
            _referenceComponents[typeof(T)] = c;
            return c;
        }

        private void TrackSync<T>()
        {
            var type = typeof(T);
            var sync = type.GetCustomAttribute(typeof(SyncedComponent)) as SyncedComponent;
            if (sync != null)
            {
                _networkedSelf.Add(type);
                if (!sync.OnlyMine)
                    _networkedPublic.Add(type);
            }
        }

        public void CallEvent(IBaseEvent ev) => _entity.Game.Systems.CallEvent(_entity, ev);

        public void RemoveComponent<T>() where T : IComponent => throw new NotImplementedException();

        public void Save<T>(in T c) where T : unmanaged, IComponent => Marshal.StructureToPtr<T>(c, _pointerComponents[c.GetType()], true);

        public bool TryGetReference<T>(out T component) where T : class, IReferenceComponent
        {
            if(_referenceComponents.TryGetValue(typeof(T), out var r))
            {
                component = (T)r;
                return true;
            }
            component = default(T);
            return false;
        }
        public T GetReference<T>() where T : class, IReferenceComponent => (T)_referenceComponents[typeof(T)];
        public unsafe T* GetPointer<T>() where T : unmanaged, IComponent => _pointerComponents.AsPointer<T>();

        public void Dispose() => _pointerComponents.FreeAll();
    }

  

}
