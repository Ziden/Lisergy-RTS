using Assets.Code.Assets.Code.Runtime;
using Assets.Code.UI;
using ClientSDK;
using Game.Engine.Events.Bus;
using Game.Systems.Party;
using Game.Tile;

/// <summary>
/// Listener for harvesting 
/// </summary>
public class HarvestingViewListener : IEventListener
{
    private IGameClient _client;
    private HarvestVfx _vfx;
    
    public HarvestingViewListener(IGameClient client)
    {
        _client = client;
        _vfx = new HarvestVfx(client);
        ClientViewState.OnSelectTile += OnSelectedTile;
        _client.ClientEvents.Register<HarvestingUpdateEvent>(this, OnHarvestResources);
    }

    private void OnHarvestResources(HarvestingUpdateEvent ev)
    {
        _ = _vfx.ShowResource(ev.Entity, ev.TileResources.Resource.ResourceId, ev.AmountHarvestedNow);
        if (ev.Depleted)
        {
            var p = ev.Entity as PartyEntity;
            _client.Game.Log.Debug($"[Harvest Prediction] Client predicted depletion on {ev.Tile} - stopping party");
            _client.Modules.Actions.StopParty(p);
            p.Components.RemoveReference<HarvestingPredictionComponent>();
         
        }
    }

    private void OnSelectedTile(TileEntity tile)
    {
        _client.UnityServices().UI.Close<ScreenTileDetails>();
        if (ClientViewState.SelectedEntityView.BaseEntity is PartyEntity party)
        {
            if (tile.HasHarvestSpot)
            {
                _client.UnityServices().UI.Open<ScreenTileDetails>(new ScreenTileDetailsParams() { Tile = tile, Harvester = party });
            }
        }
    }
}