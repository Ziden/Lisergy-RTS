using Game.ECS;
using Game.Entity;
using Game.Entity.Components;
using Game.Entity.Logic;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.Movement;
using Game.World.Components;

namespace Game.World.Systems
{
    public class EntityPlacementSystem : GameSystem<TileHabitants, Tile>
    {
        internal override void OnComponentAdded(Tile owner, TileHabitants component, EntityEventBus events)
        {
            events.RegisterComponentEvent<Tile, BuildingPlacedEvent, TileHabitants>(OnStaticEntityPlaced);
            events.RegisterComponentEvent<Tile, BuildingRemovedEvent, TileHabitants>( OnStaticEntityRemoved);
            events.RegisterComponentEvent<Tile, EntityMoveOutEvent, TileHabitants>(OnEntityMoveOut);
            events.RegisterComponentEvent<Tile, EntityMoveInEvent, TileHabitants>(OnEntityMoveIn);
        }

        private static void OnStaticEntityRemoved(Tile owner, TileHabitants component, BuildingRemovedEvent entity)
        {
            component.Building = null;
        }

        private static void OnStaticEntityPlaced(Tile owner, TileHabitants component, BuildingPlacedEvent ev)
        {
            component.Building = ev.Entity;
        }

        private static void OnEntityMoveOut(Tile owner, TileHabitants component, EntityMoveOutEvent ev)
        {
            component.EntitiesIn.Remove(ev.Entity);
        }

        private static void OnEntityMoveIn(Tile owner, TileHabitants component, EntityMoveInEvent ev)
        {
            var movement = ev.Entity.Components.Get<EntityMovementComponent>();
            if (movement == null)
            {
                return;
            }

            component.EntitiesIn.Add(ev.Entity);

            // TODO: Move to entity movement system
            if (ev.Entity is IBattleableEntity && component.Building != null && component.Building is IBattleableEntity)
            {
                if (movement.Course != null && movement.Course.Intent == MovementIntent.Offensive && movement.Course.IsLastMovement())
                {
                    StrategyGame.GlobalGameEvents.Call(new OffensiveMoveEvent()
                    {
                        Defender = component.Building,
                        Attacker = ev.Entity
                    });
                }
            }
        }
    }
}
