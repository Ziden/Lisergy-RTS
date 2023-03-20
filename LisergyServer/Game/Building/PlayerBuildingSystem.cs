using Game.ECS;
using Game.Events.GameEvents;

namespace Game.Building
{
    public class PlayerBuildingSystem : GameSystem<PlayerBuildingComponent, WorldEntity>
    {
        internal override void OnComponentAdded(WorldEntity owner, PlayerBuildingComponent component, EntityEventBus events)
        {

        }

        private static void OnPlaceBuilding(WorldEntity e, PlayerBuildingComponent c, EntityMoveInEvent ev)
        {

        }

        private static void OnRemovedBuilding(WorldEntity e, PlayerBuildingComponent c, EntityMoveOutEvent ev)
        {

        }
    }
}
