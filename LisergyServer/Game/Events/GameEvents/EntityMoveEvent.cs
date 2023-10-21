using Game.ECS;
using Game.Tile;

namespace Game.Events.GameEvents
{
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




