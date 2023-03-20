using Game.Battler;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Movement;

namespace Game.Tile
{
    public class TileHabitantsSystem : GameSystem<TileHabitants, TileEntity>
    {
        internal override void OnComponentAdded(TileEntity owner, TileHabitants component, EntityEventBus events)
        {
            events.RegisterComponentEvent<TileEntity, BuildingPlacedEvent, TileHabitants>(OnStaticEntityPlaced);
            events.RegisterComponentEvent<TileEntity, BuildingRemovedEvent, TileHabitants>(OnStaticEntityRemoved);
            events.RegisterComponentEvent<TileEntity, EntityMoveOutEvent, TileHabitants>(OnEntityMoveOut);
            events.RegisterComponentEvent<TileEntity, EntityMoveInEvent, TileHabitants>(OnEntityMoveIn);
        }

        private static void OnStaticEntityRemoved(TileEntity owner, TileHabitants component, BuildingRemovedEvent entity)
        {
            component.Building = null;
        }

        private static void OnStaticEntityPlaced(TileEntity owner, TileHabitants component, BuildingPlacedEvent ev)
        {
            component.Building = ev.Entity;
        }

        private static void OnEntityMoveOut(TileEntity owner, TileHabitants component, EntityMoveOutEvent ev)
        {
            component.EntitiesIn.Remove(ev.Entity);
        }

        private static void OnEntityMoveIn(TileEntity owner, TileHabitants component, EntityMoveInEvent ev)
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
