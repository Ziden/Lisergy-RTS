using Game.Engine.Events;
using Game.Tile;

namespace Game.Systems.Tile
{
    /// <summary>
    /// Called whenever tiledata updates for a given tile
    /// </summary>
    public class TileUpdatedEvent : IGameEvent
    {
        public TileEntity Tile;
    }
}
