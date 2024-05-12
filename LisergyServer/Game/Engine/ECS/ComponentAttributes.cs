using System;

namespace Game.ECS
{
    /// <summary>
    /// Sends component to client whenever it sends the attached entity to client.
    /// Will serialize the whole component data.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct)]
    public class SyncedComponent : Attribute
    {
        /// <summary>
        /// Only syncs for the player who is the owner of the component
        /// </summary>
        public bool OnlyMine = false;

        public SyncedComponent(bool onlyMine = false)
        {
            OnlyMine = onlyMine;
        }
    }

    /// <summary>
    /// Used on systems to determine that the given system will handle events on the client side too
    /// </summary>
    public class SyncedSystem : Attribute
    {

    }

    /// <summary>
    /// Validates the entity must have both components
    /// TODO: not implemented yet
    /// </summary>
    public class RequiresComponent : Attribute
    {

        public Type ComponentType;

        public RequiresComponent(Type componentType)
        {
            ComponentType = componentType;
        }
    }

    /// <summary>
    /// This component won't be saved during worldsave
    /// Means in case of a server restart it will be wiped
    /// </summary>
    public class NonPersisted : Attribute
    {
    }
}
