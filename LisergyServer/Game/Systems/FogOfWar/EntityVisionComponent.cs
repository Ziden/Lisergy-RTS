using Game.ECS;
using System;

namespace Game.Systems.FogOfWar
{
    [Serializable]
    [SyncedComponent(OnlyMine = true)]
    public class EntityVisionComponent : IComponent
    {
        public byte LineOfSight;
    }
}
