
using Game.ECS;
using Game.Entity.Components;
using Game.Events.GameEvents;

namespace Game.World.Systems
{
    public class PartySystem : GameSystem<PartyComponent, WorldEntity>
    {
        internal override void OnComponentAdded(WorldEntity owner, PartyComponent component, EntitySharedEventBus<WorldEntity> events)
        {
            events.RegisterComponentEvent<UnitAddedEvent, EntityExplorationComponent>(OnUnitAdded);
            events.RegisterComponentEvent<UnitRemovedEvent, EntityExplorationComponent>(OnUnitRemoved);
        }

        private static void OnUnitAdded(WorldEntity e, EntityExplorationComponent c, UnitAddedEvent ev)
        {
            c.LineOfSight = BattleGroupSystem.CalculateGetGroupLOS(e);
        }

        private static void OnUnitRemoved(WorldEntity e, EntityExplorationComponent c, UnitRemovedEvent ev)
        {
            c.LineOfSight = BattleGroupSystem.CalculateGetGroupLOS(e);
        }
    }
}
