using Game.ECS;
using Game.Events;
using Game.Tile;

namespace Game.Systems.Map
{
    /// <summary>
    /// Whenever an entity steps in a tile
    /// </summary>
    public class EntityMoveInEvent : IGameEvent
    {
        public IEntity Entity;
        public TileEntity ToTile;
        public TileEntity FromTile;

        public override string ToString()
        {
            return $"<EntityMoveIn Entity={Entity} From={FromTile} To={ToTile}/>";
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
            return $"<EntityMoveIn Entity={Entity} From={FromTile} To={ToTile}/>";
        }
    }
}
