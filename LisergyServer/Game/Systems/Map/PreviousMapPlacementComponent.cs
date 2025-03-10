using Game.Engine.ECLS;
using Game.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Systems.Map
{
    /// <summary>
    /// Refers to an entity that is placed in the map
    /// </summary>
    [Serializable]
    [SyncedComponent]
    public class PreviousMapPlacementComponent : IComponent
    {
        public Location Position;

        public override string ToString() => $"<PreviousMapPlacement {Position}>";
    }
}
