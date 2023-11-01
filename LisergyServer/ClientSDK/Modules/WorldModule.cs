using Game.Events.ServerEvents;
using ClientSDK.Data;
using Game.Tile;
using ClientSDK.SDKEvents;

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
            _client.Network.On<TileUpdatePacket>(OnReceiveTile);
        }

        private void OnReceiveTile(TileUpdatePacket packet)
        {
            var tileEntity = _client.Game.World.Map.GetTile(packet.Position.X, packet.Position.Y);
            tileEntity.UpdateData(packet.Data);
            if(packet.Components != null && packet.Components.Count > 0)
            {
                _client.Modules.Components.UpdateComponents(tileEntity, packet.Components.ToArray());
            }
            var tileView = _client.Modules.Views.GetOrCreateView(tileEntity);
            if (tileView.State == EntityViewState.NOT_RENDERED)
            {
                _client.ClientEvents.Call(new TileViewCreated()
                {
                    View = tileView,
                    Tile = tileEntity
                });
                tileView.RenderView();
            }
        }
    }
}
