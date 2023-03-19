using System.Collections.Generic;

namespace Game.Events.GameEvents
{
    /// <summary>
    /// Triggered when an entity explores or unexplores a tile.
    // Not necessarily the tile visibility changed
    /// </summary>
    public class TileExplorationStateChanged : GameEvent
    {
        public Tile Tile;
        public WorldEntity Explorer;
        public bool Explored;
    }
}
