using ClientSDK;
using ClientSDK.Data;
using Game;
using Game.ECS;
using Game.Events.Bus;
using Game.Events.GameEvents;


namespace Assets.Code.Views
{
    /// <summary>
    /// Listeners for client simulation logic events.
    /// Listens for default game logic and react to it when needed
    /// </summary>
    public class GameLogicListener : IEventListener
    {
        private IGameClient _client;

        public GameLogicListener(IGameClient c)
        {
            _client = c;
            _client.Game.Systems.TileVisibility.On<EntityTileVisibilityUpdateEvent>(OnVisibilityChange);
            Log.Debug("Registered game logic listener");
        }

        private void OnVisibilityChange(IEntity tileEntity, EntityTileVisibilityUpdateEvent ev)
        {
            if (!ev.Explorer.IsMine()) return;
           
            var view = _client.Modules.Views.GetView<TileView>(tileEntity);
            if (view == null || view.State != EntityViewState.RENDERED)
                return;

            if (ev.Explored) Log.Debug("Explored tile " + ev.Tile);
            //if (!ev.Explored) view.fog
            //else view.SetFogInTileView(false, true);
        }
    }
}
