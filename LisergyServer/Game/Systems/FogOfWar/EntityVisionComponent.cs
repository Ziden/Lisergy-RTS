using Game.ECS;
using System;

namespace Game.Systems.FogOfWar
{
    [Serializable]
    [SyncedComponent(OnlyMine = true)]
    public struct EntityVisionComponent : IComponent
    {
        public byte LineOfSight;

        public override string ToString()
        {
            return $"<EntityVisionComponent Sight={LineOfSight}>";
        }
    }
}
