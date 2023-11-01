using Assets.Code.Assets.Code.Runtime;
using Assets.Code.UI;
using ClientSDK;
using Game.ECS;
using Game.Systems.Party;
using Game.Systems.Resources;
using Game.Tile;
using GameAssets;

/// <summary>
/// Listener for harvesting 
/// </summary>
public class HarvesterListener : BaseComponentListener<HarvestingComponent>
{
    public HarvesterListener(IGameClient client) : base(client)
    {
        ClientState.OnSelectTile += OnSelectedTile;
    }

    private void OnSelectedTile(TileEntity tile)
    {
        Client.UnityServices().UI.Close<TileDetails>();
        if (ClientState.SelectedEntityView.BaseEntity is PartyEntity party)
        {
            if (party.EntityLogic.Harvesting.GetAvailableResourcesToHarvest(tile).Amount > 0)
            {
                Client.UnityServices().UI.Open<TileDetails>(new TileDetailsParams() { Tile = tile, Harvester = party });
                return;
            }
        }
    }

    public override void OnUpdateComponent(IEntity entity, HarvestingComponent oldComponent, HarvestingComponent newComponent)
    {
        if (oldComponent.StartedAt == 0 && newComponent.StartedAt > 0)
        {
            Client.UnityServices().Vfx.EntityEffects.PlayEffect(entity, MapFX.HarvestEffect);
        } else if(oldComponent.StartedAt > 0 && newComponent.StartedAt == 0)
        {
            Client.UnityServices().Vfx.EntityEffects.StopEffects(entity);
        }
    }
}