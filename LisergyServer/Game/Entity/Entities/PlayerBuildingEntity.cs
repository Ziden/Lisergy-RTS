using Game.Entity;
using Game.Entity.Components;
using Game.Events.GameEvents;
using System;

namespace Game.Entity.Entities
{

    [Serializable]
    public class PlayerBuildingEntity : WorldEntity
    {
        public ushort SpecID => Components.Get<PlayerBuildingComponent>().SpecId;

        public PlayerBuildingEntity(PlayerEntity owner) : base(owner)
        {
            this.Components.Add(new BuildingComponent());
        }

        public override string ToString()
        {
            return $"<Building Spec={SpecID} Id={Id} Owner={OwnerID}/>";
        }
    }
}
