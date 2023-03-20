using Game.ECS;
using Game.Events.GameEvents;

namespace Game.Building
{
    public class BuildingSystem : GameSystem<BuildingComponent, WorldEntity>
    {
        internal override void OnComponentAdded(WorldEntity owner, BuildingComponent component, EntityEventBus events)
        {
            events.RegisterComponentEvent<WorldEntity, EntityMoveInEvent, BuildingComponent>(OnPlaceBuilding);
            events.RegisterComponentEvent<WorldEntity, EntityMoveOutEvent, BuildingComponent>(OnRemovedBuilding);
        }

        private static void OnPlaceBuilding(WorldEntity e, BuildingComponent c, EntityMoveInEvent ev)
        {
            ev.ToTile.Components.CallEvent(new BuildingPlacedEvent(e, ev.ToTile));
        }

        private static void OnRemovedBuilding(WorldEntity e, BuildingComponent c, EntityMoveOutEvent ev)
        {
            ev.FromTile.Components.CallEvent(new BuildingRemovedEvent(e, ev.FromTile));
        }
    }
}
