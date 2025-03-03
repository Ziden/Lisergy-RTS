using Game.Engine.ECLS;
using Game.Systems.FogOfWar;
using GameData;

namespace Game.Systems.Building
{
    public unsafe class BuildingLogic : BaseEntityLogic<BuildingComponent>
    {
        public void SetupBuildingFromSpec(BuildingSpecId specId)
        {
            var spec = Game.Specs.Buildings[specId];
            var b = Entity.Components.Get<PlayerBuildingComponent>();
            b.SpecId = specId;
            Entity.Save(b);
            var c2 = Entity.Components.Get<EntityVisionComponent>();
            c2.LineOfSight = spec.LOS;
            Entity.Save(c2);
        }

        public void PlaceBuilding()
        {

        }
    }
}


