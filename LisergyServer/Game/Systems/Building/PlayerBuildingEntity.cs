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
            Components.Add<MapPlacementComponent>();
            Components.Add<BuildingComponent>();
            Components.Add<PlayerBuildingComponent>();
            Components.Add<EntityVisionComponent>();
            Components.Add<BuildingComponent>();
            Components.AddReference(new MapReferenceComponent());
        }

        public void BuildFromSpec(BuildingSpec spec)
        {
            var building = Components.Get<PlayerBuildingComponent>();
            building.SpecId = spec.Id;
            Components.Save(building);
            var vision = Components.Get<EntityVisionComponent>();
            vision.LineOfSight = spec.LOS;
            Components.Save(vision);
        }

        public override string ToString()
        {
            return $"<Building Spec={SpecId} Id={EntityId} Owner={OwnerID}/>";
        }
    }
}
