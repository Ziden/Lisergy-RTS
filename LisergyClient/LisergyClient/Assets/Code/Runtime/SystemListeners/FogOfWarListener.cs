using Assets.Code.Views;
using ClientSDK;
using ClientSDK.Data;
using Cysharp.Threading.Tasks;
using Game;
using Game.Events.Bus;
using Game.Events.GameEvents;
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

    private HashSet<TileView> _queued = new HashSet<TileView>();
    private DateTime _processingTime = DateTime.MinValue;
    private readonly TimeSpan _bufferTime = TimeSpan.FromMilliseconds(50);
    public bool Running => _processingTime != DateTime.MinValue;

    public FogOfWarListener(GameClient client)
    {
        _client = client;
        _client.Game.Events.Register<TileVisibilityChangedForPlayerEvent>(this, OnVisibilityChanged);
    }

    private void SetTileDirty(TileView view)
    {
        _queued.Add(view);
        if (!Running) _ = Run();
        else _processingTime = DateTime.UtcNow + _bufferTime;
    }

    private void OnVisibilityChanged(TileVisibilityChangedForPlayerEvent ev)
    {
        if (!ev.Explorer.OwnerID.IsMine()) return;
        var view = _client.Modules.Views.GetOrCreateView(ev.Tile);
        SetTileDirty((TileView)view);
    }

    private async UniTask Run()
    {
        _processingTime = DateTime.UtcNow + _bufferTime;
        while (DateTime.UtcNow < _processingTime)
        {
            await UniTask.Delay(10);
        }
        Log.Info($"Calculating Fog for {_queued.Count} tiles");
        var proccess = new HashSet<TileView>(_queued);
        _queued.ExceptWith(proccess);
        _processingTime = DateTime.MinValue;
        foreach (var view in proccess)
        {
            CalculateFog(view);
            await UniTask.NextFrame();
        }
    }

    public void CalculateFog(TileView view)
    {
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
        if (view.State == EntityViewState.NOT_RENDERED)
        {
            view.SetFogState(FogState.UNEXPLORED);
        }
    }

}