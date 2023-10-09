using System;

namespace Game.ECS
{
    /// <summary>
    /// Basic component interface
    /// </summary>
    public interface IComponent { }

    /// <summary>
    /// A reference component will likely not be a struct, not be serialized/saved and will be 
    /// keeping references to things for ease of use.
    /// </summary>
    public interface IReferenceComponent : IComponent { }

    /// <summary>
    /// Components only for server to hold server data.
    /// Can hold references and non-serializable data.
    /// </summary>
    public interface IServerComponent : IComponent
    {

    }
}
