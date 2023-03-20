using Game.ECS;
using System;

namespace Game.FogOfWar
{
    [Serializable]
    [SyncedComponent(OnlyMine = true)]
    public class EntityVisionComponent : IComponent
    {
        public byte LineOfSight;

        static EntityVisionComponent()
        {
            SystemRegistry<EntityVisionComponent, WorldEntity>.AddSystem(new EntityVisionSystem());
        }
    }
}
