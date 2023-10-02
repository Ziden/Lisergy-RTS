using Game.ECS;
using Game.Tile;

namespace Game.Events.GameEvents
{
    /// <summary>
    /// Changes when a TileEntity visibility changes
    /// </summary>
    public class TileVisibilityChangedEvent : GameEvent
    {
        public TileEntity Tile;
        public IEntity Explorer;
        public bool Visible;
    }
}
