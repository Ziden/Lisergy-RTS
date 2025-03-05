using Assets.Code.Views;
using ClientSDK;
using ClientSDK.Data;
using Game.Engine.Events.Bus;
using Game.Systems.FogOfWar;
using Game.World;
using System.Diagnostics;

/// <summary>
/// A fog of war lazy load. Instead of calculating the fog of war on every tile received from server
/// We try to batch this with a small time buffer to avoid re-calculating fog multiple times, we only calculate once
/// </summary>
public class FogOfWarListener : IEventListener
{
    public IGameClient _client;

    public FogOfWarListener(GameClient client)
    {
        _client = client;
        client.ClientEvents.On<TilePostRenderedEvent>(this, OnPostRender);
        client.Game.Events.On<TileVisibilityChangedEvent>(this, OnVisibilityChange);
    }

    private void OnVisibilityChange(TileVisibilityChangedEvent ev)
    {
        if (!ev.Explorer.OwnerID.IsMine()) return;

        var view = ev.Tile.Entity.GetView<TileView>();
        view.RunWhenRendered(() =>
        {
            var view = ev.Tile.Entity.GetView<TileView>();
            if (ev.Visible) view.SetFogState(FogState.EXPLORED);
            else view.SetFogState(FogState.UNEXPLORED);

            //CheckFogAround(view, Direction.EAST);
            //CheckFogAround(view, Direction.WEST);
            //CheckFogAround(view, Direction.NORTH);
            //CheckFogAround(view, Direction.SOUTH);
        });
    }

    private void OnPostRender(TilePostRenderedEvent e)
    {
        _client.Log.Debug("Post render " + e.View.Entity);
        var view = e.View;

      
    }

    private void CheckFogAround(TileView thisView, Direction d)
    {
        var near = thisView.Tile.GetNeighbor(d);
        if (near == null) return;
        var view = (TileView)_client.Modules.Views.GetOrCreateView(near.Entity);
        if (view.State == EntityViewState.NOT_RENDERED || view.GameObject == null || !view.GameObject.activeSelf || !view.Entity.IsVisible())
        {
            view.SetFogState(FogState.UNEXPLORED);
        }
    }
}