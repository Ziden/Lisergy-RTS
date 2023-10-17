using Assets.Code.Views;
using ClientSDK;
using ClientSDK.Data;
using Game;
using Game.Events.Bus;
using Game.Systems.Tile;
using Game.World;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// A fog of war lazy load. Instead of calculating the fog of war on every tile received from server
/// We try to batch this with a small time buffer to avoid re-calculating fog multiple times, we only calculate once
/// </summary>
public class FogOfWarListener : IEventListener
{
    public IGameClient _client;

    private List<TileView> _queued = new List<TileView>();
    private DateTime _processingTime = DateTime.MinValue;
    private readonly TimeSpan _bufferTime = TimeSpan.FromMilliseconds(50);
    public bool Running => _processingTime != DateTime.MinValue;

    public FogOfWarListener(GameClient client)
    {
        _client = client;
        _client.ClientEvents.Register<TileViewRendered>(this, OnTileRendered);
    }

    private void OnTileRendered(TileViewRendered e)
    {
        _queued.Add(e.View);
        if (!Running) _ = Run();
        else _processingTime = DateTime.UtcNow + _bufferTime;
    }

    private async Task Run()
    {
        _processingTime = DateTime.UtcNow + _bufferTime;
        while (DateTime.UtcNow < _processingTime)
        {
            await Task.Delay(10);
        }
        Log.Info($"Calculating Fog for {_queued.Count} tiles");
        foreach(var view in _queued)
        {
            CalculateFog(view);
        }
        _queued.Clear();
        _processingTime = DateTime.MinValue;
    }

    public void CalculateFog(TileView view)
    {
        if (view.Entity.IsVisible()) view.SetFogState(FogState.EXPLORED);
        else view.SetFogState(FogState.SEEN_BEFORE);
        CheckFogAround(view, Direction.EAST);
        CheckFogAround(view, Direction.WEST);
        CheckFogAround(view, Direction.NORTH);
        CheckFogAround(view, Direction.SOUTH);
    }

    private void CheckFogAround(TileView thisView, Direction d)
    {
        var near = thisView.Entity.GetNeighbor(d);
        var view = (TileView)_client.Modules.Views.GetOrCreateView(near);
        if (view.State == EntityViewState.NOT_RENDERED)
        {
            view.SetFogState(FogState.UNEXPLORED);
        }
    }

}