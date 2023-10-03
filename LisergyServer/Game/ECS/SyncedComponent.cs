using System;

namespace Game.ECS
{
    /// <summary>
    /// Sends component to client whenever it sends the attached entity to client.
    /// Will serialize the whole component data.
    /// </summary>
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
}
