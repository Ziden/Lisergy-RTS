using Game.ECS;
using Game.Tile;

namespace Game.Events.GameEvents
{
    /// <summary>
    /// Triggered when an entity explores or unexplores a tile.
    // Not necessarily the TileEntity visibility changed
    /// </summary>
    public class TileExplorationStateChanged : GameEvent
    {
        public TileEntity Tile;
        public IEntity Explorer;
        public bool Explored;
    }
}
