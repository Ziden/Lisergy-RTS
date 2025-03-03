using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Entities;
using Game.Systems.Building;
using Game.Systems.DeltaTracker;
using Game.Systems.Tile;
using Game.Tile;

namespace Game.Systems.Map
{
    public unsafe class MapLogic : BaseEntityLogic<MapPlaceableComponent>
    {
        public TileModel GetTile()
        {
            if (Entity.EntityType == EntityType.Tile) return Game.World.GetTile(Entity.Components.Get<TileDataComponent>().Position);
            if (!Entity.Components.TryGet<MapPlacementComponent>(out var c))
            {
                return null;
            }
            return Game.World.GetTile(c.Position);
        }

        public void RemoveEntityFromTile(IEntity e)
        {
            if (Entity.Components.TryGet<TileHabitantsComponent>(out var habitants))
            {
                if (e.Components.Has<BuildingComponent>())
                {
                    habitants.Building = null;
                }
                else
                {
                    habitants.EntitiesIn.Remove(e);
                }
                Entity.Save(habitants);
            }
        }

        public void SetEntityOnTile(IEntity steppingOn)
        {
            if (!Entity.Components.TryGet<TileHabitantsComponent>(out var habitants))
            {
                habitants = new TileHabitantsComponent();
            }
            if (steppingOn.Components.Has<BuildingComponent>())
            {
                habitants.Building = steppingOn;
            }
            else
            {
                habitants.EntitiesIn.Add(steppingOn);
            }
            Entity.Save(habitants);
        }

        public void SetPosition(TileModel newTile)
        {
            var wasPlaced = Entity.Components.TryGet<MapPlacementComponent>(out var placement);
            var hasPreviousTile = Entity.Components.TryGet<PreviousMapPlacementComponent>(out var previous);

            placement = placement ?? new MapPlacementComponent();
            previous = previous ?? new PreviousMapPlacementComponent();

            var previousTile = wasPlaced ? Game.World.GetTile(placement.Position) : null;// component.Tile;

            if (previousTile == null || newTile == null)
            {
                Entity.Logic.DeltaCompression.SetFlag(DeltaFlag.CREATED);
            }
            if (newTile != null)
            {
                placement.Position = newTile.Position;
                if (wasPlaced)
                {
                    previous.Position = previousTile.Position;
                    Entity.Save(previous);
                }
                Entity.Save(placement);
            }
            else
            {
                Entity.Components.Remove<MapPlacementComponent>();
                if (hasPreviousTile)
                {
                    Entity.Components.Remove<PreviousMapPlacementComponent>();
                }
                if (previousTile != null)
                {
                    var ev = EventPool<EntityRemovedFromMapEvent>.Get();
                    ev.Entity = Entity;
                    ev.Tile = previousTile;
                    Entity.Components.CallEvent(ev);
                    EventPool<EntityRemovedFromMapEvent>.Return(ev);
                }
            }

            Game.Log.Debug($"MapLogic - Set {Entity} position to {newTile?.Position}");
        }
    }
}
