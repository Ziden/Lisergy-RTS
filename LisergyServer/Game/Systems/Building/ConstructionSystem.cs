using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Systems.Map;

namespace Game.Systems.Building
{
    public class ConstructionSystem : LogicSystem<ConstructionWorkerComponent, ConstructionLogic>
    {
        public ConstructionSystem(LisergyGame game) : base(game) { }

        public override void RegisterListeners()
        {
            EntityEvents.On<ComponentUpdateEvent<MapPlacementComponent>>(OnPlacementUpdate);
        }

        private void OnPlacementUpdate(ComponentUpdateEvent<MapPlacementComponent> ev)
        {
            if (ev.Old != ev.New && ev.Old != null && ev.Entity.Components.TryGet<ConstructionWorkerComponent>(out var builder))
            {
                var building = World.GetTile(builder.BuildingAt).Logic.Tile.GetBuildingOnTile();
                building.Logic.Building.RemoveBuilder(ev.Entity);
            }
        }
    }
}
