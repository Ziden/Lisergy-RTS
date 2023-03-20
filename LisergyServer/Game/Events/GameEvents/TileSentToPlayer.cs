using Game.Player;
using Game.Tile;

namespace Game.Events.GameEvents
{
    /// <summary>
    /// Event whenever we need to send TileEntity information to the client
    /// </summary>
    public class TileSentToPlayerEvent : GameEvent
    {
        public TileEntity Tile;
        public PlayerEntity Player;

        public TileSentToPlayerEvent(TileEntity tile, PlayerEntity player)
        {
            Tile = tile;
            Player = player;
        }
    }
}
