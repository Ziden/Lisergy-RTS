using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Game.ECS
{
    /// <summary>
    /// Efficient way to track component delta compressions
    /// TODO: Finish to increase performance
    /// </summary>
    public unsafe class ComponentDeltaCompression
    {
        private IntPtr _ptr;
        private int _size;
        private int _offset;

        public void EnsureSpace(List<Type> components)
        {
            var totalSize = components.Sum(c => Marshal.SizeOf(c));
            if(_size < totalSize)
            {
                UnmanagedMemory.DeallocateMemory(_ptr);
                _ptr = UnmanagedMemory.Alloc(totalSize);
            }
        }

        public byte[] GetDeltaCompressed<T>(T component) where T : unmanaged, IComponent
        {
            return null;
        }
    }
}
