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

        public override void OnAdded(Tile owner, EntityPlacementComponent component, ComponentEventBus<Tile> events)
        {
            events.RegisterComponentEvent<StaticEntityPlacedEvent, EntityPlacementComponent>(this, owner, component, OnStaticEntityPlaced);
            events.RegisterComponentEvent<StaticEntityRemovedEvent, EntityPlacementComponent>(this, owner, component, OnStaticEntityRemoved);
            events.RegisterComponentEvent<EntityMoveOutEvent, EntityPlacementComponent>(this, owner, component, OnEntityMoveOut);
            events.RegisterComponentEvent<EntityMoveInEvent, EntityPlacementComponent>(this, owner, component, OnEntityMoveIn);
            events.RegisterComponentEvent<TileSentToPlayerEvent, EntityPlacementComponent>(this, owner, component, OnTileSent);
        }

        public override void OnRemoved(Tile owner, EntityPlacementComponent component, ComponentEventBus<Tile> events)
        {
           
        }

    
    }
}
