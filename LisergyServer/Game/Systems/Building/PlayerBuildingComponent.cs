using Game.ECS;
using GameData;
using System;

namespace Game.Systems.Building
{
    [Serializable]
    [SyncedComponent]
    public struct PlayerBuildingComponent : IComponent
    {
        public BuildingSpecId SpecId;
    }
}
