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
    public class EntityPlacementSystem : GameSystem<EntityPlacementComponent, Tile>
    {
        internal override void OnComponentAdded(Tile owner, EntityPlacementComponent component, EntitySharedEventBus<Tile> events)
        {
            events.RegisterComponentEvent<StaticEntityPlacedEvent, EntityPlacementComponent>(OnStaticEntityPlaced);
            events.RegisterComponentEvent<StaticEntityRemovedEvent, EntityPlacementComponent>( OnStaticEntityRemoved);
            events.RegisterComponentEvent<EntityMoveOutEvent, EntityPlacementComponent>(OnEntityMoveOut);
            events.RegisterComponentEvent<EntityMoveInEvent, EntityPlacementComponent>(OnEntityMoveIn);
            events.RegisterComponentEvent<TileSentToPlayerEvent, EntityPlacementComponent>(OnTileSent);
        }

        // TODO Make entity sent generic
        private static void OnTileSent(Tile owner, EntityPlacementComponent component, TileSentToPlayerEvent ev)
        {
            foreach (var movingEntity in component.EntitiesIn)
                ev.Player.Send(new EntityUpdatePacket(movingEntity));

            if (component.StaticEntity != null)
                ev.Player.Send(new EntityUpdatePacket(component.StaticEntity));
        }

        private static void OnStaticEntityRemoved(Tile owner, EntityPlacementComponent component, StaticEntityRemovedEvent entity)
        {
            component.StaticEntity = null;
        }

        private static void OnStaticEntityPlaced(Tile owner, EntityPlacementComponent component, StaticEntityPlacedEvent ev)
        {
            component.StaticEntity = ev.Entity;
        }

        private static void OnEntityMoveOut(Tile owner, EntityPlacementComponent component, EntityMoveOutEvent ev)
        {
            component.EntitiesIn.Remove(ev.Entity);
        }

        private static void OnEntityMoveIn(Tile owner, EntityPlacementComponent component, EntityMoveInEvent ev)
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
