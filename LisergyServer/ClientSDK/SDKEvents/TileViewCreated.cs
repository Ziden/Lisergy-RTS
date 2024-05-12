using ClientSDK.Data;
using Game.Tile;

namespace ClientSDK.SDKEvents
{
    /// <summary>
    /// Called whenever a tile view is created for the first time.
    /// This event is called before starting to render the tile view.
    /// </summary>
    public class TileViewCreated : IClientEvent
    {
        public IEntityView View;
        public TileEntity Tile;
    }
}
