using Assets.Code.Views;
using ClientSDK;
using ClientSDK.Data;
using Cysharp.Threading.Tasks;
using Game.Events.Bus;
using Game.Systems.FogOfWar;
using Game.Systems.Tile;
using Game.World;
using System;
using System.Collections.Generic;

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
        client.ClientEvents.Register<TilePostRenderedEvent>(this, OnPostRender);
        client.Game.Events.Register<TileVisibilityChangedForPlayerEvent>(this, OnVisibilityChange);
    }

    private void OnVisibilityChange(TileVisibilityChangedForPlayerEvent ev)
    {
        if (!ev.Explorer.OwnerID.IsMine()) return;

        var view = ev.Tile.GetEntityView();
        if (view == null || view.State != EntityViewState.RENDERED) return;

        if(ev.Visible) ev.Tile.GetEntityView().SetFogState(FogState.EXPLORED);
        else ev.Tile.GetEntityView().SetFogState(FogState.UNEXPLORED);
    }

    private void OnPostRender(TilePostRenderedEvent e)
    {
        var view = e.View;
        if (view.Entity.IsVisible()) view.SetFogState(FogState.EXPLORED);
        else view.SetFogState(FogState.UNEXPLORED);
        CheckFogAround(view, Direction.EAST);
        CheckFogAround(view, Direction.WEST);
        CheckFogAround(view, Direction.NORTH);
        CheckFogAround(view, Direction.SOUTH);
    }

    private void CheckFogAround(TileView thisView, Direction d)
    {
        var near = thisView.Entity.GetNeighbor(d);
        if (near == null) return;
        var view = (TileView)_client.Modules.Views.GetOrCreateView(near);
        if (view.State == EntityViewState.NOT_RENDERED) view.SetFogState(FogState.UNEXPLORED);
    }
}