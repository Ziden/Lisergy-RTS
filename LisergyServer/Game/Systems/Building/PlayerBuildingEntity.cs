using Game.Systems.FogOfWar;
using Game.Systems.Map;
using Game.Systems.MapPosition;
using Game.Systems.Player;
using GameData;
using System;

namespace Game.Systems.Building
{
    public class PlayerBuildingEntity : BaseEntity
    {
        public override EntityType EntityType => EntityType.Building;

        public ushort SpecId => Components.Get<PlayerBuildingComponent>().SpecId;

        public PlayerBuildingEntity(IGame game, PlayerEntity owner) : base(game, owner)
        {
            Components.Add(new MapPlacementComponent());
            Components.Add(new MapReferenceComponent());
            Components.Add(new BuildingComponent());
            Components.Add(new PlayerBuildingComponent());
            Components.Add(new EntityVisionComponent());
        }

        public void BuildFromSpec(BuildingSpec spec)
        {
            Components.Get<PlayerBuildingComponent>().SpecId = spec.Id;
            Components.Get<EntityVisionComponent>().LineOfSight = spec.LOS;
        }

        public override string ToString()
        {
            return $"<Building Spec={SpecId} Id={EntityId} Owner={OwnerID}/>";
        }
    }
}
