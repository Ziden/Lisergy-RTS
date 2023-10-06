using Game.ECS;
using Game.Tile;

namespace Game.Events.GameEvents
{
    public class EntityMoveInEvent : GameEvent
    {
        public IEntity Entity;
        public TileEntity ToTile;
        public TileEntity FromTile;
    }

    public class EntityMoveOutEvent : GameEvent
    {
        public IEntity Entity;
        public TileEntity ToTile;
        public TileEntity FromTile;
    }
}




