using Game.Engine.ECLS;
using System;

namespace Game.Systems.FogOfWar
{
    /// <summary>
    /// Defines that this entity is able to see things on the map
    /// Its line of sight will reveal tiles around this entity for its owner
    /// </summary>
    [Serializable]
    [SyncedComponent(OnlyMine = true)]
    public class EntityVisionComponent : IComponent
    {
        public byte LineOfSight;

        public override string ToString()
        {
            return $"<EntityVisionComponent Sight={LineOfSight}>";
        }
    }
}
