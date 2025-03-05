using Game.Engine.DataTypes;
using Game.Engine.Events;
using Game.Systems.DeltaTracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Game.Engine.ECLS
{
    public class ComponentSet
    {
        private static Dictionary<Type, SyncedComponent> _shouldSync = new Dictionary<Type, SyncedComponent>();
        private static List<IComponent> _returnBuffer = new List<IComponent>(); // TODO: Multi-thread
        internal Dictionary<Type, IComponent> _components;
        internal HashSet<Type> _saved;
        internal Dictionary<Type, IComponent> _read;
        internal HashSet<Type> _removed;
        internal IEntity _entity;

        public ComponentSet(IEntity e)
        {
            _entity = e;
        }

        public ComponentSet()
        {
        }

        public void SetOwner(IEntity e)
        {
            _entity = e;
        }

        private ICollection<Type> GetComponentsToSync(bool onlyDeltas)
        {
            if (onlyDeltas) return GetModified();
            return _components.Keys;
        }

        /// <summary>
        /// TODO: Use proper buffers for performance
        /// </summary>
        public (List<IComponent> updated, HashSet<Type> removed) GetComponentDeltas(GameId receiver = default, bool deltaCompression = true)
        {
            _returnBuffer.Clear();
            var toSync = GetComponentsToSync(deltaCompression);
            foreach (var kp in toSync)
            {
                if (ShouldSync(kp, receiver)) _returnBuffer.Add(GetByType(kp));
            }
            return (_returnBuffer, GetRemoved());
        }

        public HashSet<Type> GetModified()
        {
            _saved = _saved ?? new HashSet<Type>();
            return _saved;
        }

        public HashSet<Type> GetRemoved()
        {
            _removed = _removed ?? new HashSet<Type>();
            return _removed;
        }
        public Dictionary<Type, IComponent> GetComponents()
        {
            _components = _components ?? new Dictionary<Type, IComponent>();
            return _components;
        }

        public Dictionary<Type, IComponent> GetReadCopies()
        {
            _read = _read ?? new Dictionary<Type, IComponent>();
            return _read;
        }

        public void ClearDeltas()
        {
            GetModified().Clear();
            GetRemoved().Clear();
            GetReadCopies().Clear();
        }

        public IReadOnlyCollection<Type> AllTypes()
        {
            return GetComponents().Keys;
        }

        public IReadOnlyCollection<IComponent> AllComponents()
        {
            return GetComponents().Values;
        }

        public bool HasDeltas() => GetModified().Count > 0 || GetRemoved().Count > 0;

        public bool Has<T>() where T : IComponent
        {
            var t = typeof(T);
            return GetComponents().ContainsKey(t);
        }

        public void Add<T>(T obj = default) where T : IComponent, new()
        {
            var t = typeof(T);
            GetComponents()[t] = t.IsValueType ? default : obj == null ? FastNew<T>.Instance() : obj;
            var ev = ClassPool<ComponentUpdateEvent<T>>.Get();
            ev.Entity = _entity;
            ev.Old = default;
            ev.New = (T)GetComponents()[t];
            CallEvent(ev);
            ClassPool<ComponentUpdateEvent<T>>.Return(ev);

            OnAfterAdded(t);
        }

        public void OnAfterAdded(Type t)
        {
            TrackSync(t);
            FlagCompnentHasDelta(t);
        }

        public bool IsSyncableComponent(Type t)
        {
            if(!_entity.Game.Network.DeltaCompression.Enabled) return false;
            if (!_shouldSync.TryGetValue(t, out var sync)) return false;
            if (sync == null) return false;
            return true;
        }

        public bool ShouldSync(Type t, GameId to)
        {
            var sync = _shouldSync[t];
            if (sync == null) return false;
            if (sync.OnlyMine && to != _entity.OwnerID) return false;
            return true;
        }

        private void FlagCompnentHasDelta(Type t)
        {
            if (!IsSyncableComponent(t)) return;
            if (GetModified().Add(t))
            {
                _entity.Logic.DeltaCompression.SetFlag(DeltaFlag.COMPONENTS);
            }
        }

        public bool Remove<T>() where T : IComponent
        {
            var t = typeof(T);
            if (GetComponents().TryGetValue(t, out var c))
            {
                if (c is IDisposable d) d.Dispose();
                if (GetComponents().Remove(t))
                {
                    GetRemoved().Add(t);
                    _entity.Logic.DeltaCompression.SetFlag(DeltaFlag.COMPONENTS);

                    GetModified().Remove(t);
                    _entity.Game.Log.Debug($"Removed {t.Name} from {_entity}");

                    var ev = ClassPool<ComponentUpdateEvent<T>>.Get();
                    ev.Entity = _entity;
                    ev.Old = (T)c;
                    ev.New = default;
                    CallEvent(ev);
                    ClassPool<ComponentUpdateEvent<T>>.Return(ev);

                    return true;
                }
            }
            return false;
        }

        private void TrackSync(Type type)
        {
            if (!_entity.Game.Network.DeltaCompression.Enabled) return;
            if (!_shouldSync.TryGetValue(type, out var sync))
            {
                sync = type.GetCustomAttribute(typeof(SyncedComponent)) as SyncedComponent;
                _shouldSync[type] = sync;
            }
        }

        public bool TryGet<T>(out T comp) where T : IComponent
        {
            var t = typeof(T);
            var readCopies = GetReadCopies();
            var has = GetComponents().TryGetValue(t, out var currentComponent);
            if (has)
            {
                if (IsSyncableComponent(t))
                {
                    if (!readCopies.ContainsKey(t))
                    {
                        readCopies[t] = currentComponent.FastShallowClone();
                    }
                    comp = (T)readCopies[t];
                    return true;
                }
                comp = (T)currentComponent;
            }
            else
            {
                comp = default;
            }
            return has;
        }

        public void CallEvent(IBaseEvent ev) => _entity.Game.Logic.Systems.CallEvent(_entity, ev);

        public bool CompareWith<T>(IEntity otherEntity) where T : IComponent
        {
            var other = otherEntity.Get<T>();
            var mine = Get<T>();
            var myBytes = Serialization.FromAnyType(mine);
            var hisBytes = Serialization.FromAnyType(other); // TODO: implement fast comparison in components
            return myBytes.SequenceEqual(hisBytes);
        }

        public bool IsUpToDateWith(IEntity otherEntity)
        {
            foreach (var c in _components)
            {
                if (!IsSyncableComponent(c.Key)) continue;

                var other = otherEntity.Components.GetByType(c.Value.GetType());
                var mine = c.Value;
                var myBytes = Serialization.FromAnyType(mine);
                var hisBytes = Serialization.FromAnyType(other); // TODO: implement fast comparison in components
                var equal = myBytes.SequenceEqual(hisBytes);
                if (!equal)
                {
                    _entity.Game.Log.Error($"{_entity} desync component: {c.Value}");
                    return false;
                }
            }
            return true;

        }

        public void Save<T>(in T c) where T : IComponent
        {
            var t = c.GetType();
            GetComponents().TryGetValue(t, out var oldValue);

            var ev = ClassPool<ComponentUpdateEvent<T>>.Get();
            ev.Entity = _entity;
            ev.Old = (T)oldValue;
            ev.New = c;
            CallEvent(ev);
            ClassPool<ComponentUpdateEvent<T>>.Return(ev);

            GetComponents()[t] = c;
            if (t.IsValueType)
            {
                GetReadCopies()[t] = c;
            }
            else
            {
                GetReadCopies().Remove(t);
            }
            TrackSync(t);
            FlagCompnentHasDelta(t);
        }

        public T Get<T>() where T : IComponent
        {
            var r = GetByType(typeof(T));
            return r == null ? default : (T)r;
        }

        public IComponent GetByType(Type t)
        {
            GetComponents().TryGetValue(t, out var c);
            var readCopy = GetReadCopies();
            if (IsSyncableComponent(t))
            {
                readCopy[t] = c?.FastShallowClone(); // TODO: Maybe only tests always returns a copy to ensure .Save is being called ?
                c = readCopy[t];
            }
            return c == null ? default : c;
        }

        public void ValidateComponentSetModifications()
        {
            /*
            foreach (var currentType in GetModified())
            {
                GetComponents().TryGetValue(currentType, out var current);
                if (currentType != null)
                {
                    var currentBytes = Serialization.FromAnyType(current);
                    var previousBytes = _read[currentType];
                    var previous = Serialization.ToAnyType<IComponent>(previousBytes);
                    if (!currentBytes.SequenceEqual(previousBytes))
                    {
                        throw new Exception($"Entity {_entity} had modified component {previous} that was not properly saved");
                    }
                    else
                    {
                        Console.WriteLine("OK");
                    }
                }
            }
            */
        }

        public override string ToString()
        {
            return $"<Components Size={GetComponents().Count} [{string.Join(',', GetComponents().Keys.Select(k => k.Name))}]>";
        }
    }
}
