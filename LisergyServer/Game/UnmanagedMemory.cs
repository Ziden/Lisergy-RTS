using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Game
{
    public static class UnmanagedMemory
    {

        private static Dictionary<IntPtr, int> _allocs = new Dictionary<IntPtr, int>();

        public static IntPtr Allocate(int size)
        {
            var p = Marshal.AllocHGlobal(size);
            _allocs[p] = size;
            return p;
        }

        public static void Free(IntPtr p)
        {
            var size = _allocs[p];
            Marshal.FreeHGlobal(p);
            _allocs.Clear();
        }

        public static void Free()
        {
            foreach(var p in _allocs.Keys)
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
                $"Total Unmanaged Size: {_allocs.Values.Sum()} KB"
            };
        }
    }
}
