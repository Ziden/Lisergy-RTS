using Game.Tile;

namespace Game.Events.GameEvents
{
    public class EntityMoveInEvent : GameEvent
    {
        public WorldEntity Entity;
        public TileEntity ToTile;
        public TileEntity FromTile;
    }

    public class EntityMoveOutEvent : GameEvent
    {
        public WorldEntity Entity;
        public TileEntity ToTile;
        public TileEntity FromTile;
    }
}




