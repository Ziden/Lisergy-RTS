using Assets.Code.Assets.Code.Runtime;
using Assets.Code.UI;
using ClientSDK;
using Game.Events.Bus;
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
    }

    private void OnSelectedTile(TileEntity tile)
    {
        _client.UnityServices().UI.Close<WidgetTileDetails>();
        if (ClientViewState.SelectedEntityView.BaseEntity is PartyEntity party)
        {
            if (tile.HasHarvestSpot)
            {
                _client.UnityServices().UI.Open<WidgetTileDetails>(new TileDetailsParams() { Tile = tile, Harvester = party });
            }
        }
    }
}