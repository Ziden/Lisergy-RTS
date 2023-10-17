using Game.Events;
using Game.Systems.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.ECS
{
    /// <summary>
    /// Holds the components of a given entity.
    /// Can have two type of component, pointers and references.
    /// Pointers are the real data components where reference components act like a "cache" of references
    /// to other objects.
    /// </summary>
    public unsafe interface IComponentSet : IDisposable
    {
        /// <summary>
        /// Gets all components a given player needs to have in sync for this given entity
        /// </summary>
        IReadOnlyList<IComponent> GetSyncedComponents(PlayerEntity receiver, bool deltaCompression = true);

        /// <summary>
        /// Gets all pointer components this entity have
        /// </summary>
        IReadOnlyCollection<Type> All();

        /// <summary>
        /// Saves a component that was modified by reference
        /// </summary>
        void Save<T>(in T c) where T : IComponent;

        /// <summary>
        /// Adds a new component to the entity
        /// </summary>
        void Add<T>() where T : unmanaged, IComponent;

        /// <summary>
        /// Gets a component from the entity.
        /// Will get the reference of the component, but if the component is assigned to
        /// another variable or passed down by parameter without REF or IN keywords it will create a copy.
        /// </summary>
        ref T Get<T>() where T : unmanaged, IComponent;

        /// <summary>
        /// Gets a pointer component by type
        /// </summary>
        IComponent GetByType(Type t);

        /// <summary>
        /// Gets a pointer for a component.
        /// This allows component data to be modified more easily.
        /// </summary>
        T* GetPointer<T>() where T : unmanaged, IComponent;
        T AddReference<T>(in T c) where T : class, IReferenceComponent;
        T GetReference<T>() where T : class, IReferenceComponent;
        bool TryGetReference<T>(out T component) where T : class, IReferenceComponent;
        bool TryGet<T>(out T component) where T : unmanaged, IComponent;
        bool Has<T>() where T : unmanaged, IComponent;
        void ClearDeltas();
        void CallEvent(IBaseEvent e);
    }

}
