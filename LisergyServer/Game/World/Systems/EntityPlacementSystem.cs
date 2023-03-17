using Game.Battle;
using Game.ECS;
using Game.Entity;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.Movement;
using Game.World.Components;

namespace Game.World.Systems
{
    public class EntityPlacementSystem : GameSystem<EntityPlacementComponent, Tile>
    {
        public override void OnComponentAdded(Tile owner, EntityPlacementComponent component, EntitySharedEventBus<Tile> events)
        {
            events.RegisterComponentEvent<StaticEntityPlacedEvent, EntityPlacementComponent>(OnStaticEntityPlaced);
            events.RegisterComponentEvent<StaticEntityRemovedEvent, EntityPlacementComponent>( OnStaticEntityRemoved);
            events.RegisterComponentEvent<EntityMoveOutEvent, EntityPlacementComponent>(OnEntityMoveOut);
            events.RegisterComponentEvent<EntityMoveInEvent, EntityPlacementComponent>(OnEntityMoveIn);
            events.RegisterComponentEvent<TileSentToPlayerEvent, EntityPlacementComponent>(OnTileSent);
        }

        public void OnTileSent(Tile owner, EntityPlacementComponent component, TileSentToPlayerEvent ev)
        {
            foreach (var movingEntity in component.EntitiesIn)
                ev.Player.Send(new EntityUpdatePacket(movingEntity));

            if (component.StaticEntity != null)
                ev.Player.Send(new EntityUpdatePacket(component.StaticEntity));
        }

        public void OnStaticEntityRemoved(Tile owner, EntityPlacementComponent component, StaticEntityRemovedEvent entity)
        {
            component.StaticEntity = null;
        }

        public void OnStaticEntityPlaced(Tile owner, EntityPlacementComponent component, StaticEntityPlacedEvent ev)
        {
            component.StaticEntity = ev.Entity;
        }

        public void OnEntityMoveOut(Tile owner, EntityPlacementComponent component, EntityMoveOutEvent ev)
        {
            component.EntitiesIn.Remove(ev.Entity);
        }

        public void OnEntityMoveIn(Tile owner, EntityPlacementComponent component, EntityMoveInEvent ev)
        {
            var entity = ev.Entity as MovableWorldEntity;
            component.EntitiesIn.Add(entity);


            var tile = ev.Entity.Tile;

            var same = tile == owner;

            if (entity is IBattleable && component.StaticEntity != null && component.StaticEntity is IBattleable)
            {
                if (entity.Course != null && entity.Course.Intent == MovementIntent.Offensive && entity.Course.IsLastMovement())
                {
                    entity.Tile.Game.GameEvents.Call(new OffensiveMoveEvent()
                    {
                        Defender = component.StaticEntity,
                        Attacker = entity
                    });
                }
            }
        }
    }
}
