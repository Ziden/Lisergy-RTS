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

        internal HashSet<Type> _modifiedComponents = new HashSet<Type>();
        internal List<Type> _networkedPublic = new List<Type>();
        internal List<Type> _networkedSelf = new List<Type>();

        internal IEntity _entity;

        private List<IComponent> _returnBuffer = new List<IComponent>();

        public ComponentSet(IEntity entity)
        {
            _entity = entity;
        }

        /// <summary>
        /// TODO: Use buffers for performance
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyList<IComponent> GetSyncedComponents(PlayerEntity receiver, bool deltaCompression = true)
        {
            Log.Debug($"Sync component for entity {_entity}");
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
            return _returnBuffer;
        }

        public void ClearDeltas() => _modifiedComponents.Clear();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyCollection<Type> All() => _pointerComponents.Keys;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get<T>() where T : unmanaged, IComponent => ref _pointerComponents.AsReference<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>() where T : unmanaged, IComponent => _pointerComponents.ContainsKey(typeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasReference<T>() where T : class, IComponent => _referenceComponents.ContainsKey(typeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<T>() where T : unmanaged, IComponent
        {
            _modifiedComponents.Add(typeof(T));
            _pointerComponents.Alloc<T>();
            TrackSync<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T AddReference<T>(in T c) where T : class, IReferenceComponent
        {
            _referenceComponents[typeof(T)] = c;
            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CallEvent(IBaseEvent ev) => _entity.Game.Systems.CallEvent(_entity, ev);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent<T>() where T : IComponent => throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Save<T>(in T c) where T : IComponent
        {
            Marshal.StructureToPtr<T>(c, _pointerComponents[c.GetType()], true);
            _modifiedComponents.Add(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetReference<T>() where T : class, IReferenceComponent => (T)_referenceComponents[typeof(T)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe T* GetPointer<T>() where T : unmanaged, IComponent
        {
            _modifiedComponents.Add(typeof(T));
            return _pointerComponents.AsPointer<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => _pointerComponents.FreeAll();

        public IComponent GetByType(Type t) => _pointerComponents.AsInterface(t);
    }



}
