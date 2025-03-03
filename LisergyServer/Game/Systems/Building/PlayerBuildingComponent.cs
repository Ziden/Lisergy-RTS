using Game.Engine.ECLS;
using GameData;
using System;

namespace Game.Systems.Building
{
    [Serializable]
    [SyncedComponent]
    public class PlayerBuildingComponent : IComponent
    {
        public BuildingSpecId SpecId;
    }
}
