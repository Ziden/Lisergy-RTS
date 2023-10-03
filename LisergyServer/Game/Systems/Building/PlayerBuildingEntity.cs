using Game.Systems.Player;
using System;

namespace Game.Systems.Building
{

    [Serializable]
    public class PlayerBuildingEntity : BaseEntity
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
