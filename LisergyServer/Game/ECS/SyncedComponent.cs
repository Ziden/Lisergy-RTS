using System;

namespace Game.ECS
{
    /// <summary>
    /// Sends component to client whenever it sends the attached entity to client.
    /// Will serialize the whole component data.
    /// If the component defines a logic type, then the code will look for the logic type in the entity to sync instead
    /// of a simple serialization 
    /// </summary>
    public class SyncedComponent : Attribute
    {

        /// <summary>
        /// If entity shares the logic as sync logic using 
        /// IEntitySyncsLogic and a type matches with this attribute it will use that type to sync this component
        /// </summary>
        public Type LogicPropSyncType;

        /// <summary>
        /// Only syncs for the player who is the owner of the component
        /// </summary>
        public bool OnlyMine = false;

        public SyncedComponent(Type logicProperties = null, bool onlyMine = false)
        {
            LogicPropSyncType = logicProperties;
            OnlyMine = onlyMine;
        }
    }
}
