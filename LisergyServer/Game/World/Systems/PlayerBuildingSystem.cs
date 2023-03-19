using Game.ECS;
using Game.Events.GameEvents;
using Game.World.Components;

namespace Game.World.Systems
{
    public class PlayerBuildingSystem : GameSystem<PlayerBuildingComponent, Tile>
    {
        internal override void OnComponentAdded(Tile owner, PlayerBuildingComponent component, EntitySharedEventBus<Tile> events)
        {
            events.RegisterComponentEvent<BuildingPlacedEvent, TileHabitants>(OnStaticEntityPlaced);
            events.RegisterComponentEvent<BuildingRemovedEvent, TileHabitants>( OnStaticEntityRemoved);
        }

        private static void OnStaticEntityRemoved(Tile owner, TileHabitants component, BuildingRemovedEvent entity)
        {
            component.Building = null;
        }

        private static void OnStaticEntityPlaced(Tile owner, TileHabitants component, BuildingPlacedEvent ev)
        {
            component.Building = ev.Entity;
        }
    }
}
