using Game.ECS;
using Game.Entity.Systems;
using Game.World.Systems;
using System;

namespace Game.Entity.Components
{
    [Serializable]
    [SyncedComponent]
    public class PlayerBuildingComponent : IComponent
    {
        public ushort SpecId;

        static PlayerBuildingComponent()
        {
            SystemRegistry<PlayerBuildingComponent, WorldEntity>.AddSystem(new PlayerBuildingSystem());
        }
    }
}
