using System.Collections.Generic;

namespace Game.Events.GameEvents
{
    public class TileVisibilityChangedEvent : GameEvent
    {
        public Tile Tile;
        public WorldEntity Explorer;
        public bool Visible;
    }
}
