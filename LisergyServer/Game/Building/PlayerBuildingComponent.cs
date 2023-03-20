using Game.ECS;
using System;

namespace Game.Building
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
