using Game.ECS;
using Game.Systems.Map;

namespace Game.Systems.Building
{
    public class BuildingSystem : GameSystem<BuildingComponent>
    {
        public BuildingSystem(LisergyGame game) : base(game) { }

        public override void OnEnabled()
        {
            EntityEvents.On<EntityMoveInEvent>(OnPlaceBuilding);
            EntityEvents.On<EntityMoveOutEvent>(OnRemovedBuilding);
        }

        private void OnPlaceBuilding(IEntity e, EntityMoveInEvent ev)
        {
            ev.ToTile.Components.CallEvent(new BuildingPlacedEvent(e, ev.ToTile));
        }

        private void OnRemovedBuilding(IEntity e, EntityMoveOutEvent ev)
        {
            ev.FromTile.Components.CallEvent(new BuildingRemovedEvent(e, ev.FromTile));
        }
    }
}
