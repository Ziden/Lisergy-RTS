using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Game.ECS
{
    /// <summary>
    // Class to manage pointers to components.
    // All data components are stored in unmanaged memory for speed and better memory usage.
    /// </summary>
    [DebuggerTypeProxy(typeof(ComponentPointersDebugView))]
    public unsafe class ComponentPointers : Dictionary<Type, IntPtr>
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IntPtr Pointer<T>() => this[typeof(T)];

        /// <summary>
        /// Gets the component as a reference.
        /// Any modifications to the component using the reference variable will be done on
        /// the actual memory-space of the component.
        /// 
        /// If the component is assigned to another variable tho, it will copy its data.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T AsReference<T>() where T : unmanaged => ref *(T*)Pointer<T>();

        /// <summary>
        /// Gets a hard pointer to the component. Any modifications or passing down as parameters will still modify the component memory space.
        /// This is a faster operation than using the component by reference
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* AsPointer<T>() where T : unmanaged => (T*)Pointer<T>();

        /// <summary>
        /// Reads the given component as IComponent base interface
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IComponent AsInterface(Type t) => (IComponent)Marshal.PtrToStructure(this[t], t);

        /// <summary>
        /// Allocates unmanaged memory for the given component to exist.
        /// Will attempt to reuse any free memory if available.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Alloc<T>() where T : unmanaged => this[typeof(T)] = UnmanagedMemory.Alloc(sizeof(T));

        /// <summary>
        /// Free's allocated memory for the given component
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free<T>() where T : unmanaged
        {
            UnmanagedMemory.Free(Pointer<T>());
            Remove(typeof(T));
        }

        /// <summary>
        /// Free's all memory used by this entity components
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FreeAll()
        {
            foreach (var p in this.Values) UnmanagedMemory.Free(p);
        }

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
