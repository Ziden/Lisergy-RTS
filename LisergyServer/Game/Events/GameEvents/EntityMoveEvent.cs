using Game.Tile;

namespace Game.Events.GameEvents
{
    public class EntityMoveInEvent : GameEvent
    {
        public BaseEntity Entity;
        public TileEntity ToTile;
        public TileEntity FromTile;
    }

    public class EntityMoveOutEvent : GameEvent
    {
        public BaseEntity Entity;
        public TileEntity ToTile;
        public TileEntity FromTile;
    }
}




