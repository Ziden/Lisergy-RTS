
using Game.ECS;
using Game.Entity.Components;
using Game.Events.GameEvents;

namespace Game.World.Systems
{
    public class EntityMovementSystem : GameSystem<EntityMovementComponent , WorldEntity>
    {
        internal override void OnComponentAdded(WorldEntity owner, EntityMovementComponent component, EntitySharedEventBus<WorldEntity> events)
        {
            //events.RegisterComponentEvent<TileVisibilityChangedEvent, TileVisibilityComponent>(OnTileVisibilityUpdated);
        }

        public void MoveEntity(WorldEntity entity, Tile otherTile)
        {
            entity.Tile = otherTile;
            var oldTile = entity.Tile;
            if (oldTile != null)
            {
                oldTile.Components.CallEvent(new EntityMoveOutEvent()
                {
                    Entity = entity,
                    ToTile = otherTile,
                    FromTile = oldTile
                });
            }
            if (otherTile != null)
            {
                otherTile.Components.CallEvent(new EntityMoveInEvent()
                {
                    Entity = entity,
                    ToTile = otherTile,
                    FromTile = oldTile
                });
            }

            Log.Info($"Placed {this} in {otherTile}");
        }
    }
}
