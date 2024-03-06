using Game.Engine.ECS;
using Game.Engine.Events;
using Game.Tile;

namespace Game.Systems.FogOfWar
{
    /// <summary>
    /// Triggered when an entity explores or unexplores a tile.
    // Not necessarily the TileEntity visibility changed
    /// </summary>
    public class EntityTileVisibilityUpdateEvent : IGameEvent
    {
        public TileEntity Tile;
        public IEntity Explorer;
        public bool Explored;

        public override string ToString()
        {
            return $"<TileExplorationStateChanged {Tile} to {Explored}>";
        }
    }

    /// <summary>
    /// Changes when a TileEntity visibility changes
    /// </summary>
    public class TileVisibilityChangedForPlayerEvent : IGameEvent
    {
        public TileEntity Tile;
        public IEntity Explorer;
        public bool Visible;

        public override string ToString()
        {
            return $"<TileVisibilityChange {Tile} to {Visible}>";
        }
    }
}
