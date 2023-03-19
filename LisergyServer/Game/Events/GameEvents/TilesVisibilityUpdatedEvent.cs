using System.Collections.Generic;

namespace Game.Events.GameEvents
{
    /// <summary>
    /// Changes when a tile visibility changes
    /// </summary>
    public class TileVisibilityChangedEvent : GameEvent
    {
        public Tile Tile;
        public WorldEntity Explorer;
        public bool Visible;
    }
}
