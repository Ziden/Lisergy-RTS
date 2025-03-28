using ClientSDK;
using Cysharp.Threading.Tasks;
using Game.Engine.Events.Bus;
using Game.Systems.FogOfWar;
using Game.Tile;
using Godot;
using GodotClient.Services;
using LisergyGodotClient.Src.Services;
using LisergyGodotClient.Src.Systems.GameHud;

namespace LisergyGodotClient.Src.Systems.Tiles
{
    public class TileListener : IEventListener
    {
        private IClientStateService _state;
        private IUiService _ui;
        private IClientSDK _sdk;
        private TileFog _fog;

        public TileListener()
        {
            _fog = new TileFog(100, 100, ClientServices.Get<IGameObject>().GetNode<Node>());
            _ui = ClientServices.Get<IUiService>();
            _state = ClientServices.Get<IClientStateService>();
            _sdk = ClientServices.Get<IClientSDK>();
            _state.OnTileSelected += State_OnTileSelected;    
            _state.OnCameraMoved += State_OnCameraMoved;
            _sdk.ClientEvents.On<ClientPartyActionEvent>(this, OnPartyAction);
            _sdk.Game.Events.On<TileVisibilityChangedEvent>(this, OnTileVisibility);
        }

        private void OnTileVisibility(TileVisibilityChangedEvent e)
        {
            _fog.SetVisible(e.Tile.Entity.GetNode(), e.Visible);
            var building = e.Tile.Logic.Tile.GetBuildingOnTile();
            var entities = e.Tile.Logic.Tile.GetEntitiesOnTile();
            if (building != null)
            {
                _fog.SetVisible(building.GetNode(), e.Visible);
            }
            foreach(var entity in entities)
            {
                if(!entity.IsMine())
                {
                    _fog.SetVisible(entity.GetNode(), e.Visible);
                }
            }
        }

        private void OnPartyAction(ClientPartyActionEvent e)
        {
            if(e.Action == EntityAction.CHECK && e.TargetTile != null)
            {
                _ui.Open<TileDetailsScreen>().ContinueWith(screen =>
                {
                    screen.SetData(e.TargetTile);
                });
            }
        }

        private void State_OnCameraMoved(Vector3 vector)
        {
            _ui.Close<TileDetailsScreen>();
        }

        private void State_OnTileSelected(TileModel model)
        {
           
        }
    }
}
