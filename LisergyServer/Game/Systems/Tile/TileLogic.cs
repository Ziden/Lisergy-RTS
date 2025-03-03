using Game.Engine.ECLS;
using Game.Engine.Events;
using GameData;
using System;
using System.Collections.Generic;

namespace Game.Systems.Tile
{
    public class TileLogic : BaseEntityLogic<TileDataComponent>
    {
        public IReadOnlyCollection<IEntity> GetEntitiesOnTile()
        {
            var tile = Game.World.GetTile(Entity.Get<TileDataComponent>().Position);
            var habitants = tile.Components.Get<TileHabitantsComponent>();
            if (habitants != null) return habitants.EntitiesIn;
            return Array.Empty<IEntity>();
        }

        public void SetTileId(TileSpecId id)
        {
            var data = Entity.Get<TileDataComponent>();
            var tile = Game.World.GetTile(data.Position);
            data.TileId = id;
            tile.Save(data);
            var ev = EventPool<TileUpdatedEvent>.Get();
            ev.Tile = tile;
            tile.Components.CallEvent(ev);
            EventPool<TileUpdatedEvent>.Return(ev);
        }

        public bool IsPassable() => GetMovementFactor() > 0;
        public float GetMovementFactor() => Game.Specs.Tiles[Entity.Get<TileDataComponent>().TileId].MovementFactor;

        public IEntity GetBuildingOnTile()
        {
            var tile = Game.World.GetTile(Entity.Get<TileDataComponent>().Position);
            var habitants = tile.Components.Get<TileHabitantsComponent>();
            return habitants?.Building;
        }
    }
}
