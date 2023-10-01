using Game.ECS;
using Game.Events.GameEvents;

namespace Game.Systems.Building
{
    public class BuildingSystem : GameSystem<BuildingComponent, WorldEntity>
    {
        public override void OnEnabled()
        {
            SystemEvents.On<EntityMoveInEvent>(OnPlaceBuilding);
            SystemEvents.On<EntityMoveOutEvent>(OnRemovedBuilding);
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
