using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Game
{
    public static class UnmanagedMemory
    {

        private static readonly Dictionary<IntPtr, int> _allocs = new Dictionary<IntPtr, int>();

        private static Dictionary<IntPtr, int> _available = new Dictionary<IntPtr, int>();

#if WINDOWS
        [DllImport("kernel32.dll")]
        static extern void RtlZeroMemory(IntPtr dst, UIntPtr length);
#endif

        public static void SetZeros(IntPtr ptr, int size)
        {
#if WINDOWS
            UIntPtr usize = new UIntPtr((uint)size);
            RtlZeroMemory(ptr, usize);
#else
            Marshal.Copy(new byte[size], 0, ptr, size);
#endif
        }

        public static IntPtr Alloc(int size)
        {
            IntPtr available = GetAvailable(size);
            if (available.ToInt64() > 0)
            {
                _ = _available.Remove(available);
                _allocs[available] = size;
                SetZeros(available, size);
                return available;
            }
            IntPtr p = Marshal.AllocHGlobal(size);
            _allocs[p] = size;
            SetZeros(p, size);
            return p;
        }

        private static IntPtr GetAvailable(int size)
        {
            KeyValuePair<IntPtr, int> kp = _available.FirstOrDefault(kp => kp.Value == size);
            return kp.Key;
        }

        /// <summary>
        /// Flag memory to be reused. Mainly for tests or reuse map server to spawn different map;
        /// </summary>
        public static void FlagMemoryToBeReused()
        {
            _available = new Dictionary<IntPtr, int>(_allocs);
            _allocs.Clear();
        }

        public static void FlagMemoryToBeReused(IntPtr ptr)
        {
            if (_allocs.ContainsKey(ptr))
            {
                _available[ptr] = _allocs[ptr];
                _ = _allocs.Remove(ptr);
            }
        }


        public static void Free(IntPtr p)
        {
            _ = _allocs[p];
            Marshal.FreeHGlobal(p);
            _allocs.Clear();
        }

        public static void FreeAll()
        {
            foreach (IntPtr p in _allocs.Keys)
            {
                Marshal.FreeHGlobal(p);
            }
            _allocs.Clear();
        }

        public static string[] GetMetrics()
        {
            return new string[]
            {
                $"Unmanaged Pointers: {_allocs.Count}",
                $"Total Unmanaged Size: {_allocs.Values.Sum()/1024} KB"
            };
        }
    }
}
