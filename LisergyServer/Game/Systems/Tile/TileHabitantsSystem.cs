using Game.ECS;
using Game.Events.GameEvents;
using Game.Systems.Battler;
using Game.Systems.Movement;
using Game.Tile;

namespace Game.Systems.Tile
{
    public class TileHabitantsSystem : GameSystem<TileHabitants, TileEntity>
    {
        public override void OnEnabled()
        {
            SystemEvents.On<BuildingPlacedEvent>(OnStaticEntityPlaced);
            SystemEvents.On<BuildingRemovedEvent>(OnStaticEntityRemoved);
            SystemEvents.On<EntityMoveOutEvent>(OnEntityMoveOut);
            SystemEvents.On<EntityMoveInEvent>(OnEntityMoveIn);
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
