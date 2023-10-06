using Game.ECS;
using Game.Events.GameEvents;

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

        private static void OnPlaceBuilding(IEntity e, BuildingComponent c, EntityMoveInEvent ev)
        {
            ev.ToTile.Components.CallEvent(new BuildingPlacedEvent(e, ev.ToTile));
        }

        private static void OnRemovedBuilding(IEntity e, BuildingComponent c, EntityMoveOutEvent ev)
        {
            ev.FromTile.Components.CallEvent(new BuildingRemovedEvent(e, ev.FromTile));
        }
    }
}
