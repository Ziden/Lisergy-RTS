using Game.ECS;
using NetSerializer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Game.Engine.ECS
{
    /// <summary>
    // Class to manage pointers to components.
    // All data components are stored in unmanaged memory for speed and better memory usage.
    /// </summary>
    [DebuggerTypeProxy(typeof(ComponentPointersDebugView))]
    public unsafe class ComponentPointers : Dictionary<Type, IntPtr>
    {
        /// <summary>
        /// Try to get a pointer component as a struct
        /// </summary>

        public bool TryGet<T>(out T outPtr) where T : unmanaged, IComponent
        {
            if (!TryGetValue(typeof(T), out var ptr))
            {
                outPtr = default;
                return false;
            }
            outPtr = *(T*)ptr;
            return true;
        }

        private IntPtr Pointer<T>() => this[typeof(T)];

        /// <summary>
        /// Gets the component as a reference.
        /// Any modifications to the component using the reference variable will be done on
        /// the actual memory-space of the component.
        /// 
        /// If the component is assigned to another variable tho, it will copy its data.
        /// </summary>

        public ref T AsReference<T>() where T : unmanaged => ref *(T*)Pointer<T>();

        /// <summary>
        /// Gets a hard pointer to the component. Any modifications or passing down as parameters will still modify the component memory space.
        /// This is a faster operation than using the component by reference
        /// </summary>

        public T* AsPointer<T>() where T : unmanaged => (T*)Pointer<T>();

        /// <summary>
        /// Reads the given component as IComponent base interface
        /// </summary>

        public IComponent AsInterface(Type t)
        {
            if (!TryGetValue(t, out var ptr)) return null;
            return (IComponent)Marshal.PtrToStructure(ptr, t);
        }

        /// <summary>
        /// Allocates unmanaged memory for the given component to exist.
        /// Will attempt to reuse any free memory if available.
        /// </summary>

        public void Alloc<T>() where T : IComponent => this[typeof(T)] = UnmanagedMemory.Alloc(Marshal.SizeOf<T>());

        public void Alloc(Type t) => this[t] = UnmanagedMemory.Alloc(Marshal.SizeOf(t));

        /// <summary>
        /// Free's allocated memory for the given component
        /// </summary>

        public void Free<T>() where T : unmanaged
        {
            UnmanagedMemory.FreeForReuse(Pointer<T>());
            Remove(typeof(T));
        }

        public void Free(Type t)
        {
            UnmanagedMemory.FreeForReuse(this[t]);
            Remove(t);
        }

        /// <summary>
        /// Free's all memory used by this entity components
        /// </summary>

        public void FreeAll()
        {
            foreach (var p in Values) UnmanagedMemory.DeallocateMemory(p);
        }

        /// <summary>
        /// Gets all components
        /// </summary>
        public IComponent[] ToArray() => Keys.Select(type => AsInterface(type)).ToArray();

        /// <summary>
        /// Debug panel to be able to visualize the pointers in debugger windows
        /// </summary>
        internal class ComponentPointersDebugView
        {
            private ComponentPointers _pointers;
            public ComponentPointersDebugView(ComponentPointers p) { _pointers = p; }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public IReadOnlyCollection<IComponent> Components => _pointers.Keys.Select(t => _pointers.AsInterface(t)).ToList();
        }
    }
}
