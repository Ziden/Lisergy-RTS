using System;

namespace Game.ECS
{
    /// <summary>
    /// Basic component interface. Will likely implement component as an unmanaged struct with
    /// only unmanaged types and no references.
    /// Will be stored and read like a pointer.
    /// If need to keep references, ideally should be using <see cref="IReferenceComponent"/>
    /// </summary>
    public interface IComponent { }

    /// <summary>
    /// A reference component that is stored as a class and can have references to other objects. 
    /// This is to store dynamic data that holds references and not only struct data.
    /// </summary>
    public interface IReferenceComponent : IComponent { }
}
