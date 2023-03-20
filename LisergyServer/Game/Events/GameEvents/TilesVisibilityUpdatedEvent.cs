using Game.Tile;

namespace Game.Events.GameEvents
{
    /// <summary>
    /// Changes when a TileEntity visibility changes
    /// </summary>
    public class TileVisibilityChangedEvent : GameEvent
    {
        public TileEntity Tile;
        public WorldEntity Explorer;
        public bool Visible;
    }
}
