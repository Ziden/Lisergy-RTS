using System;
using System.Collections.Generic;
using System.Text;

namespace Game.ECS
{

    /// <summary>
    /// Sends component to client whenever it sends the attached entity to client.
    /// Will serialize the whole component data
    /// </summary>
    public class SyncedComponent : Attribute { }
}
