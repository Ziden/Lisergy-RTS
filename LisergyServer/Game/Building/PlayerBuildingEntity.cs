using Game.Player;
using System;

namespace Game.Building
{

    [Serializable]
    public class PlayerBuildingEntity : WorldEntity
    {
        public ushort SpecID => Components.Get<PlayerBuildingComponent>().SpecId;

        public PlayerBuildingEntity(PlayerEntity owner) : base(owner)
        {
            Components.Add(new BuildingComponent());
        }

        public override string ToString()
        {
            return $"<Building Spec={SpecID} Id={Id} Owner={OwnerID}/>";
        }
    }
}
