using ClientSDK.Services;
using Game.Engine.ECLS;
using Game.Systems.FogOfWar;
using Game.Systems.Map;

namespace ClientSDK.Sync;

/// <summary>
///     Module responsible for handling base component logic to keep the client logic "up to date"
///     This module single responsibility is to try to keep the client game in sync with server game as most as it can
///     The only thing this won't keep track is the player data which is handled in <see cref="IPlayerModule" />
/// </summary>
public class SystemSynchronizer
{
	private readonly LisergySDK _gameClient;

	public SystemSynchronizer(LisergySDK gameClient)
	{
		_gameClient = gameClient;
	}

	public void ListenForRequiredSyncs()
	{
		_gameClient.Server.Entities.OnComponentModified<MapPlacementComponent>(OnUpdatePlacement);
		_gameClient.Server.Entities.OnComponentAdded<MapPlacementComponent>(OnAddPlacement);
	}

	private void OnAddPlacement(IEntity entity, MapPlacementComponent component)
	{
		var tile = _gameClient.Game.World.GetTile(component.Position);
		entity.Logic.Map.SetPosition(tile);
		if (entity.Components.Has<EntityVisionComponent>() && entity.OwnerID == _gameClient.Server.Player.PlayerId)
			entity.Logic.Vision.UpdateVisionRange(null, tile);
	}

    /// <summary>
    ///     Whenever map placement updates are sent to the client we re-position the entity on the client logic.
    ///     This will trigger all exploration events
    /// </summary>
    private void OnUpdatePlacement(IEntity entity, MapPlacementComponent oldValue, MapPlacementComponent newValue)
	{
		if (!entity.Components.Has<EntityVisionComponent>() ||
		    entity.OwnerID != _gameClient.Server.Player.PlayerId) return;
		if (oldValue.Position != newValue.Position)
		{
			var oldTile = _gameClient.Game.World.GetTile(oldValue.Position.X, oldValue.Position.Y);
			var newTile = _gameClient.Game.World.GetTile(newValue.Position.X, newValue.Position.Y);
			entity.Logic.Vision.UpdateVisionRange(oldTile, newTile);
		}
	}
}