using System;

namespace Game.ECS
{
    public interface IComponent
    {
    }

    /// <summary>
    /// Components only for server to hold server data.
    /// Can hold references and non-serializable data.
    /// </summary>
    public interface IServerComponent : IComponent
    {

    }
}
