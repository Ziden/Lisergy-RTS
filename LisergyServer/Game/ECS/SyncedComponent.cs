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

        public Type LogicType;

        public SyncedComponent(Type logicType = null)
        {
            LogicType = logicType;
        }
    }
}
