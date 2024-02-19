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
        internal HashSet<Type> _removedComponents = new HashSet<Type>();

        internal HashSet<Type> _networkedPublic = new HashSet<Type>();
        internal HashSet<Type> _networkedPrivate = new HashSet<Type>();

        internal IEntity _entity;

        private List<IComponent> _returnBuffer = new List<IComponent>();

        public ComponentPointers Pointers => _pointerComponents;

        public ComponentSet(IEntity entity)
        {
            _entity = entity;
        }

        /// <summary>
        /// TODO: Use proper buffers for performance
        /// </summary>

        public (List<IComponent> updated, HashSet<Type> removed) GetComponentDeltas(PlayerEntity receiver, bool deltaCompression = true)
        {
            _returnBuffer.Clear();
            if (receiver != null && receiver.OwnerID == _entity.OwnerID)
            {
                foreach (var t in _networkedPrivate)
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
            return (_returnBuffer, _removedComponents);
        }

        public void ClearDeltas()
        {
            _entity.DeltaFlags.Clear();
            _modifiedComponents.Clear();
            _removedComponents.Clear();
        }

        public IReadOnlyCollection<Type> All() => _pointerComponents.Keys;

        public ref T Get<T>() where T : unmanaged, IComponent => ref _pointerComponents.AsReference<T>();

        public bool HasDeltas() => _modifiedComponents.Count > 0 || _removedComponents.Count > 0;

        public bool Has<T>() where T : unmanaged, IComponent => _pointerComponents.ContainsKey(typeof(T));


        public bool HasReference<T>() where T : class, IComponent => _referenceComponents.ContainsKey(typeof(T));


        public void Add<T>() where T : unmanaged, IComponent
        {
            Add(typeof(T));
        }

        public void Add(Type t)
        {
            TrackSync(t);
            FlagCompnentHasDelta(t);
            _pointerComponents.Alloc(t);
        }

        private void FlagCompnentHasDelta(Type t)
        {
            if (_networkedPrivate.Contains(t))
            {
                _modifiedComponents.Add(t);
                _entity.DeltaFlags.SetFlag(Network.DeltaFlag.COMPONENTS);
            }
        }

        public void Remove(Type t)
        {
            _pointerComponents.Free(t);
            _removedComponents.Add(t);
            _modifiedComponents.Remove(t);
            _entity.DeltaFlags.SetFlag(Network.DeltaFlag.COMPONENTS);
            UntrackSync(t);
            _entity.Game.Log.Debug($"Removed {t} from {_entity}");
        }

        public void Remove<T>() where T : unmanaged, IComponent
        {
            var t = typeof(T);

            _pointerComponents.Free<T>();
            if (_networkedPrivate.Contains(t))
            {
                _removedComponents.Add(t);
                _entity.DeltaFlags.SetFlag(Network.DeltaFlag.COMPONENTS);
            }
            UntrackSync(t);
            _modifiedComponents.Remove(t);
            _entity.Game.Log.Debug($"Removed {typeof(T)} from {_entity}");
        }

        public T AddReference<T>(in T c) where T : class, IReferenceComponent
        {
            _referenceComponents[typeof(T)] = c;
            return c;
        }

        public void RemoveReference<T>() where T : class, IReferenceComponent
        {
            var t = typeof(T);
            if (_referenceComponents.TryGetValue(t, out var c))
            {
                if (c is IDisposable d) d.Dispose();
                _referenceComponents.Remove(t);
            }
        }

        private void TrackSync(Type type)
        {
            var sync = type.GetCustomAttribute(typeof(SyncedComponent)) as SyncedComponent;
            if (sync != null)
            {
                _networkedPrivate.Add(type);
                if (!sync.OnlyMine)
                    _networkedPublic.Add(type);
            }
        }

        private void UntrackSync(Type type)
        {
            var sync = type.GetCustomAttribute(typeof(SyncedComponent)) as SyncedComponent;
            if (sync != null)
            {
                _networkedPrivate.Remove(type);
                if (!sync.OnlyMine)
                    _networkedPublic.Remove(type);
            }
        }

        public bool TryGet<T>(out T comp) where T : unmanaged, IComponent => _pointerComponents.TryGet(out comp);

        public void CallEvent(IBaseEvent ev) => _entity.Game.Systems.CallEvent(_entity, ev);

        public void Save<T>(in T c) where T : IComponent
        {
            var t = c.GetType();
            if (!_pointerComponents.TryGetValue(t, out var ptr))
            {
                _pointerComponents.Alloc(t);
                TrackSync(t);
            }
            Marshal.StructureToPtr(c, _pointerComponents[t], true);
            FlagCompnentHasDelta(t);
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
            var t = typeof(T);
            FlagCompnentHasDelta(t);
            return _pointerComponents.AsPointer<T>();
        }


        public void Dispose() => _pointerComponents.FreeAll();

        public IComponent GetByType(Type t) => _pointerComponents.AsInterface(t);


    }



}
