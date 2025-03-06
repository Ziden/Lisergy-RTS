using Assets.Code.Assets.Code.Runtime;
using Assets.Code.UI;
using ClientSDK;
using Game.Engine.Events.Bus;
using Game.Entities;
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
        _client.ClientEvents.On<HarvestingUpdateEvent>(this, OnHarvestResources);
    }

    private void OnHarvestResources(HarvestingUpdateEvent ev)
    {
        _ = _vfx.ShowResource(ev.Entity, ev.TileResources.Resource.ResourceId, ev.AmountHarvestedNow);
        if (ev.Depleted)
        {
            var p = ev.Entity;
            _client.Game.Log.Debug($"[Harvest Prediction] Client predicted depletion on {ev.Tile} - stopping party");
            _client.Modules.Actions.StopEntity(p);
            p.Components.Remove<HarvestingPredictionComponent>();
        }
    }

    private void OnSelectedTile(TileModel tile)
    {
        _client.UnityServices().UI.Close<ScreenTileDetails>();
        var selected = ClientViewState.SelectedEntityView.Entity;
        if (selected?.EntityType == EntityType.Party)
        {
            if (tile.HasHarvestSpot)
            {
                _client.UnityServices().UI.Open<ScreenTileDetails>(new ScreenTileDetailsParams() { Tile = tile, Harvester = selected });
            }
        }
    }
}