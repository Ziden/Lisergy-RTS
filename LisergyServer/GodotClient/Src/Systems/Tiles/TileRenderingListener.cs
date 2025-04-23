using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientSDK;
using Game.Engine.Events;
using Game.Engine.Events.Bus;
using LisergyGodotClient.Data;
using LisergyGodotClient.Src;

/// <summary>
///     A pipeline to proccess tile decoration.
///     It will batch any upcoming tiles to avoid double processing as tiles are coming from server
/// </summary>
public class TileRenderingListener : IAutoRegisterListener
{
	private readonly TimeSpan _bufferTime = TimeSpan.FromMilliseconds(20);

	private readonly HashSet<TileRenderedEvent> _queued = new();
	private DateTime _bufferExpireTime = DateTime.MinValue;
	public IClientSdk _client;
	
	public void OnRegister()
	{
		_client = ClientServices.Get<IClientSdk>();
		_client.ClientEvents.On<TileRenderedEvent>(this, OnTileRendered);
	}

	public bool Running => _bufferExpireTime != DateTime.MinValue;

	private void OnTileRendered(TileRenderedEvent ev)
	{
		SetTileDirty(ev);
	}

	/// <summary>
	///     Sets the tile as having visibility modified.
	///     It will get buffered to be recalculated on the next batch
	/// </summary>
	private void SetTileDirty(TileRenderedEvent ev)
	{
		_queued.Add(ev);
		if (!Running) _ = WaitForNextBatch();
		else _bufferExpireTime = DateTime.UtcNow + _bufferTime;
	}

	private async Task WaitForNextBatch()
	{
		_bufferExpireTime = DateTime.UtcNow + _bufferTime;
		while (DateTime.UtcNow < _bufferExpireTime) await Task.Delay(10);
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