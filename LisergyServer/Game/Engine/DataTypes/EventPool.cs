using Game.Engine.DataTypes;
using System;
using System.Collections.Generic;

namespace Game.Engine.Events
{

    public class EventPoolValidator
    {
        public static HashSet<Type> Used = new HashSet<Type>();

        public static void ValidateDisposed()
        {
            if (Used.Count > 0)
            {
                throw new Exception("Some events have not been returned to pool: " + string.Join(',', Used));
            }
        }
    }

    public struct ClassPool<T> where T : new()
    {
        private static Queue<T> _free = new Queue<T>();
        private static HashSet<T> _used = new HashSet<T>();

        public static int GetUsed() => _used.Count;

        public static bool IsUsed(T o) => _used.Contains(o);

        private static void FlagUsed(T item)
        {
            _used.Add(item);
        }

        public static T Get()
        {
            if (_free.TryDequeue(out var item))
            {
                FlagUsed(item);
                return item;
            }
            item = FastNew<T>.Instance();
            FlagUsed(item);
            return item;
        }

        public static void Return(T item)
        {
            _free.Enqueue(item);
            _used.Remove(item);
        }
    }

    public static class EventPool<T> where T : new()
    {
        private static Queue<T> _free = new Queue<T>();
        private static HashSet<T> _used = new HashSet<T>();

        public static int MAX_CREATED = 1;

        public static int GetUsed() => _used.Count;

        private static void FlagUsed(T item)
        {
            _used.Add(item);
#if DEBUG
            EventPoolValidator.Used.Add(item.GetType());
#endif
        }

        public static T Get()
        {
            if (_free.TryDequeue(out var item))
            {
                FlagUsed(item);
                return item;
            }
            item = FastNew<T>.Instance();
            FlagUsed(item);
            if (_used.Count > 1)
            {
                throw new Exception($"Leak on {item?.GetType()}");
            }
            return item;
        }

        public static void Return(T item)
        {
            _free.Enqueue(item);
            _used.Remove(item);
#if DEBUG
            EventPoolValidator.Used.Remove(item?.GetType());
#endif
        }
    }
}
