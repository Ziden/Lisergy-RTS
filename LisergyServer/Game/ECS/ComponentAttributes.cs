using System;
using System.Collections.Generic;
using System.Text;

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
}
