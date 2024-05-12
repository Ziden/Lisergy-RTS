using Game.Events.ServerEvents;
using ClientSDK.Data;
using Game.Tile;
using ClientSDK.SDKEvents;
using System;

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
        private GameClient _client;

        public WorldModule(GameClient client)
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
            if(packet.Components != null && packet.Components.Length > 0)
            {
                _client.Modules.Components.ProccessUpdate(tileEntity, packet.Components, Array.Empty<uint>());
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
