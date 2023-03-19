using Game.Battle;
using Game.ECS;
using Game.Entity;
using Game.Entity.Components;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.Movement;
using Game.World.Components;

namespace Game.World.Systems
{
    public class EntityPlacementSystem : GameSystem<TileHabitants, Tile>
    {
        internal override void OnComponentAdded(Tile owner, TileHabitants component, EntitySharedEventBus<Tile> events)
        {
            events.RegisterComponentEvent<StaticEntityPlacedEvent, TileHabitants>(OnStaticEntityPlaced);
            events.RegisterComponentEvent<StaticEntityRemovedEvent, TileHabitants>( OnStaticEntityRemoved);
            events.RegisterComponentEvent<EntityMoveOutEvent, TileHabitants>(OnEntityMoveOut);
            events.RegisterComponentEvent<EntityMoveInEvent, TileHabitants>(OnEntityMoveIn);
        }

        private static void OnStaticEntityRemoved(Tile owner, TileHabitants component, StaticEntityRemovedEvent entity)
        {
            component.StaticEntity = null;
        }

        private static void OnStaticEntityPlaced(Tile owner, TileHabitants component, StaticEntityPlacedEvent ev)
        {
            component.StaticEntity = ev.Entity;
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
            if (ev.Entity is IBattleable && component.StaticEntity != null && component.StaticEntity is IBattleable)
            {
                if (movement.Course != null && movement.Course.Intent == MovementIntent.Offensive && movement.Course.IsLastMovement())
                {
                    StrategyGame.GlobalGameEvents.Call(new OffensiveMoveEvent()
                    {
                        Defender = component.StaticEntity,
                        Attacker = ev.Entity
                    });
                }
            }
        }
    }
}
