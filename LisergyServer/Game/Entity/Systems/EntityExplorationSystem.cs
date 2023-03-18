
using Game.ECS;
using Game.Entity.Components;

namespace Game.World.Systems
{
    public class EntityExplorationSystem : GameSystem<EntityExplorationComponent , WorldEntity>
    {
        internal override void OnComponentAdded(WorldEntity owner, EntityExplorationComponent component, EntitySharedEventBus<WorldEntity> events)
        {

            //events.RegisterComponentEvent<TileVisibilityChangedEvent, TileVisibilityComponent>(OnTileVisibilityUpdated);
        }
    }
}
