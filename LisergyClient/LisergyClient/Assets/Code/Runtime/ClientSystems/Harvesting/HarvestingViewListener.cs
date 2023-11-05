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
        _client.ClientEvents.Register<HarvestedResourceEvent>(this, OnHarvestResources);
    }

    private void OnHarvestResources(HarvestedResourceEvent ev)
    {
        _ = _vfx.ShowResource(ev.Entity, ev.TileResources.ResourceId, ev.AmountHarvestedNow);
    }

    private void OnSelectedTile(TileEntity tile)
    {
        _client.UnityServices().UI.Close<WidgetTileDetails>();
        if (ClientViewState.SelectedEntityView.BaseEntity is PartyEntity party)
        {
            if (party.EntityLogic.Harvesting.GetAvailableResourcesToHarvest(tile).Amount > 0)
            {
                _client.UnityServices().UI.Open<WidgetTileDetails>(new TileDetailsParams() { Tile = tile, Harvester = party });
            }
        }
    }
}