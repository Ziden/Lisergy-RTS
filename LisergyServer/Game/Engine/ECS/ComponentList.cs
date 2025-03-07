using Game.Engine.ECLS;
using System;
using System.Collections.Generic;

namespace Game.Engine.ECS
{
    public class ComponentList
    {
        private ComponentPointers _pointers = new ComponentPointers();
        private Dictionary<Type, IGameComponent> _references = new Dictionary<Type, IGameComponent>();

        public IEnumerable<Type> Keys => _references.Keys;

        public IEnumerable<IGameComponent> Values => _references.Values;

        public IGameComponent this[Type t]
        {
            get
            {
                return _references[t];
            }
            set
            {
                _references[t] = value;
            }
        }

        public bool Remove(Type t)
        {
            if (_pointers.ContainsKey(t))
            {
                _pointers.Free(t);
                return true;
            }
            return _references.Remove(t);

        }

        public bool ContainsKey(Type t) => _references.ContainsKey(t);

        public bool TryGetValue(Type t, out IGameComponent c)
        {
            return _references.TryGetValue(t, out c);
        }

        public int Count => _references.Count;
    }
}
