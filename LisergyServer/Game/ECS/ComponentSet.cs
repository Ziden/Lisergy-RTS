using Game.Events;
using Game.Systems.Building;
using Game.Systems.Player;
using System;
using System.Collections.Generic;
using System.Linq;
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

        internal HashSet<Type> _modifiedComponents = new HashSet<Type>();
        internal HashSet<IComponent> _removedComponents = new HashSet<IComponent>();
        internal List<Type> _networkedPublic = new List<Type>();
        internal List<Type> _networkedSelf = new List<Type>();

        internal IEntity _entity;

        private List<IComponent> _returnBuffer = new List<IComponent>();

        public ComponentPointers Pointers => _pointerComponents;

        public ComponentSet(IEntity entity)
        {
            _entity = entity;
        }

        /// <summary>
        /// TODO: Use buffers for performance
        /// TODO: Communicate removed components
        /// </summary>
       
        public IReadOnlyList<IComponent> GetSyncedComponents(PlayerEntity receiver, bool deltaCompression = true)
        {
            _returnBuffer.Clear();
            if (receiver != null && receiver.OwnerID == _entity.OwnerID)
            {
                foreach (var t in _networkedSelf)
                {
                    if (!deltaCompression || _modifiedComponents.Contains(t)) _returnBuffer.Add(_pointerComponents.AsInterface(t));
                }
            }
            else
            {
                foreach (var t in _networkedPublic)
                {
                    if (!deltaCompression || _modifiedComponents.Contains(t))
                        _returnBuffer.Add(_pointerComponents.AsInterface(t));
                }
            }
            foreach (var r in _removedComponents) _returnBuffer.Add(r);
            _entity.Game.Log.Debug($"Sync components [{string.Join(",",_returnBuffer.Select(c => c.GetType().Name))}] for entity {_entity}");
            return _returnBuffer;
        }

        public void ClearDeltas()
        {
            _modifiedComponents.Clear();
            _removedComponents.Clear();
        }

       
        public IReadOnlyCollection<Type> All() => _pointerComponents.Keys;

       
        public ref T Get<T>() where T : unmanaged, IComponent => ref _pointerComponents.AsReference<T>();

       
        public bool Has<T>() where T : unmanaged, IComponent => _pointerComponents.ContainsKey(typeof(T));

       
        public bool HasReference<T>() where T : class, IComponent => _referenceComponents.ContainsKey(typeof(T));

       
        public void Add<T>() where T : unmanaged, IComponent
        {
            _modifiedComponents.Add(typeof(T));
            _pointerComponents.Alloc<T>();
            TrackSync<T>();
        }

        public void Remove<T>() where T : unmanaged, IComponent
        {
            var t = typeof(T);
            _modifiedComponents.Remove(t);
            _pointerComponents.Free<T>();
            _removedComponents.Add(default(T));
            UntrackSync<T>();
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

        private void UntrackSync<T>()
        {
            var type = typeof(T);
            var sync = type.GetCustomAttribute(typeof(SyncedComponent)) as SyncedComponent;
            if (sync != null)
            {
                _networkedSelf.Remove(type);
                if (!sync.OnlyMine)
                    _networkedPublic.Remove(type);
            }
        }

        public bool TryGet<T>(out T comp) where T : unmanaged, IComponent => _pointerComponents.TryGet<T>(out comp);

        public void CallEvent(IBaseEvent ev) => _entity.Game.Systems.CallEvent(_entity, ev);

        public void RemoveComponent<T>() where T : IComponent => throw new NotImplementedException();
       
        public void Save<T>(in T c) where T : IComponent
        {
            Marshal.StructureToPtr<T>(c, _pointerComponents[c.GetType()], true);
            _modifiedComponents.Add(typeof(T));
        }

       
        public bool TryGetReference<T>(out T component) where T : class, IReferenceComponent
        {
            if (_referenceComponents.TryGetValue(typeof(T), out var r))
            {
                component = (T)r;
                return true;
            }
            component = default(T);
            return false;
        }

       
        public T GetReference<T>() where T : class, IReferenceComponent => (T)_referenceComponents[typeof(T)];

       
        public unsafe T* GetPointer<T>() where T : unmanaged, IComponent
        {
            _modifiedComponents.Add(typeof(T));
            return _pointerComponents.AsPointer<T>();
        }

       
        public void Dispose() => _pointerComponents.FreeAll();

        public IComponent GetByType(Type t) => _pointerComponents.AsInterface(t);
    }



}
