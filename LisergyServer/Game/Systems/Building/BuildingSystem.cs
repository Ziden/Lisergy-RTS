using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Systems.Map;

namespace Game.Systems.Building
{
    [SyncedSystem]
    public class BuildingSystem : LogicSystem<BuildingComponent, BuildingLogic>
    {
        public BuildingSystem(LisergyGame game) : base(game) { }

        public override void RegisterListeners()
        {
            //EntityEvents.On<ComponentUpdateEvent<MapPlacementComponent>>(OnPlacementUpdate);
        }

        private void OnPlacementUpdate(IEntity e, ComponentUpdateEvent<MapPlacementComponent> ev)
        {

        }
    }
}
