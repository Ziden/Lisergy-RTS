using Game.ECS;
using Game.Events;
using Game.Tile;

namespace Game.Systems.Map
{
    /// <summary>
    /// Whenever an entity moves in a tile
    /// </summary>
    public class EntityMoveInEvent : IGameEvent
    {
        public IEntity Entity;
        public TileEntity ToTile;
        public TileEntity FromTile;

        public override string ToString()
        {
            return $"<EntityMoveIn Entity={Entity} From={FromTile.Position} To={ToTile.Position}/>";
        }
    }

    /// <summary>
    /// Whenever an entity steps out of a tile
    /// </summary>
    public class EntityMoveOutEvent : IGameEvent
    {
        public IEntity Entity;
        public TileEntity ToTile;
        public TileEntity FromTile;

        public override string ToString()
        {
            return $"<EntityMoveIn Entity={Entity} From={FromTile.Position} To={ToTile.Position}/>";
        }
    }
}
