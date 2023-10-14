using Game.DataTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Game.Events
{
    public static class EventPool<T> where T : new()
    {
        private static Queue<T> _free = new Queue<T>();
        public static int MAX_CREATED = 1;
        private static int _created = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get()
        {
            if (_free.TryDequeue(out var item)) return item;
            _created++;
            if (_created > MAX_CREATED) throw new Exception("Some leak with object " + typeof(T).Name);
            return FastNew<T>.Instance();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Return(T item) {
            _free.Enqueue(item);
            _created--;
        }
    }
}
