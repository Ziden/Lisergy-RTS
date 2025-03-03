using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Tile;

namespace Game.Systems.Map
{

    public class EntityPlacedInMapEvent : IGameEvent
    {
        public IEntity Entity;
        public TileModel ToTile;

        public override string ToString()
        {
            return $"<EntityPlaced Entity={Entity} To={ToTile.Position}/>";
        }
    }

    public class EntityRemovedFromMapEvent : IGameEvent
    {
        public IEntity Entity;
        public TileModel Tile;

        public override string ToString()
        {
            return $"<EntityRemovedFromMapEvent Entity={Entity} To={Tile.Position}/>";
        }
    }
}
