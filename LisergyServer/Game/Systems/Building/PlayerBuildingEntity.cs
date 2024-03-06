using Game.Engine.DataTypes;
using Game.Systems.FogOfWar;
using Game.Systems.MapPosition;
using GameData;

namespace Game.Systems.Building
{
    public unsafe class PlayerBuildingEntity : BaseEntity
    {
        public override EntityType EntityType => EntityType.Building;

        public BuildingSpecId SpecId => Components.Get<PlayerBuildingComponent>().SpecId;

        public PlayerBuildingEntity(IGame game, GameId owner) : base(game, owner)
        {
            Components.AddReference(new MapReferenceComponent());
        }

        public void BuildFromSpec(BuildingSpec spec)
        {
            Components.GetPointer<PlayerBuildingComponent>()->SpecId = spec.SpecId;
            Components.GetPointer<EntityVisionComponent>()->LineOfSight = spec.LOS;
        }

        public override string ToString()
        {
            return $"<Building Spec={SpecId} Id={EntityId} Owner={OwnerID}/>";
        }
    }
}
