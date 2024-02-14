using Assets.Code.Views;
using ClientSDK;
using Cysharp.Threading.Tasks;
using Game.Events;
using Game.Events.Bus;
using System;
using System.Collections.Generic;

/// <summary>
/// A pipeline to proccess tile decoration.
/// It will batch any upcoming tiles to avoid double processing as tiles are coming from server
/// </summary>
public class TileRenderingListener : IEventListener
{
    public IGameClient _client;

    private HashSet<TileRenderedEvent> _queued = new HashSet<TileRenderedEvent>();
    private DateTime _bufferExpireTime = DateTime.MinValue;
    private readonly TimeSpan _bufferTime = TimeSpan.FromMilliseconds(20);
    public bool Running => _bufferExpireTime != DateTime.MinValue;

    public TileRenderingListener(GameClient client)
    {
        _client = client;
        _client.ClientEvents.Register<TileRenderedEvent>(this, OnTileRendered);
    }

    private void OnTileRendered(TileRenderedEvent ev)
    {
        SetTileDirty(ev);
    }

    /// <summary>
    /// Sets the tile as having visibility modified. 
    /// It will get buffered to be recalculated on the next batch
    /// </summary>
    private void SetTileDirty(TileRenderedEvent ev)
    {
        _queued.Add(ev);
        if (!Running) _ = WaitForNextBatch();
        else _bufferExpireTime = DateTime.UtcNow + _bufferTime;
    }

    private async UniTask WaitForNextBatch()
    {
        _bufferExpireTime = DateTime.UtcNow + _bufferTime;
        while (DateTime.UtcNow < _bufferExpireTime)
        {
            await UniTask.Delay(10);
        }
        _client.Log.Debug($"Post Processing {_queued.Count} tiles");
        var proccess = new HashSet<TileRenderedEvent>(_queued);
        _queued.ExceptWith(proccess);
        _bufferExpireTime = DateTime.MinValue;
        var e = EventPool<TilePostRenderedEvent>.Get();
        foreach (var ev in proccess)
        {
            if (ev.View.GameObject == null) continue;
            e.Reactivate = ev.Reactivate;
            e.View = ev.View;
            _client.ClientEvents.Call(e);
        }
        EventPool<TilePostRenderedEvent>.Return(e);
    }
}