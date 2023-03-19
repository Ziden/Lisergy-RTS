using System;
using System.Collections.Generic;
using System.Text;

namespace Game.ECS
{

    /// <summary>
    /// Validates the entity must have both components
    /// TODO: not implemented yet
    /// </summary>
    public class RequiresComponent : Attribute {

        public Type ComponentType;

        public RequiresComponent(Type componentType)
        {
            ComponentType = componentType;
        }
    }
}
