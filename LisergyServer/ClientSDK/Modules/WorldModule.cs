using Game.Events.ServerEvents;
using ClientSDK.Data;
using Game.Tile;

namespace ClientSDK.Services
{
    /// <summary>
    /// Service responsible for handling authentication and specific account and profile information
    /// </summary>
    public interface IWorldModule : IClientModule
    {
    }

    public class WorldModule : IWorldModule
    {
        private IGameClient _client;

        public WorldModule(IGameClient client)
        {
            _client = client;
        }

        public void Register()
        {
            _client.Network.On<TilePacket>(OnReceiveTile);
        }

        private void OnReceiveTile(TilePacket tile)
        {
            var tileEntity = _client.Game.World.Map.GetTile(tile.Data.X, tile.Data.Y);
            tileEntity.UpdateData(tile.Data);
            var tileView = _client.Modules.Views.GetOrCreateView(tileEntity);
            if(tileView.State == EntityViewState.NOT_RENDERED) tileView.RenderView();
        }
    }
}
