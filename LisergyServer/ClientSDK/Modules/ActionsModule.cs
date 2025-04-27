using System.Linq;
using ClientSDK.Autoplay;
using ClientSDK.Data;
using ClientSDK.SDKEvents;
using Game.Engine.ECLS;
using Game.Systems.Building;
using Game.Systems.Course;
using Game.Systems.Harvesting;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.Tile;
using Game.World;
using GameData;

namespace ClientSDK.Services;

/// <summary>
///     Module for player input actions. This module encapsulates all potential actions the player can do on the game using
///     network
/// </summary>
public interface IActionModule : IClientModule
{
    /// <summary>
    ///     Tries to move the given entity to the target destination.
    ///     Will return true or false if the entity is able to move there or not.
    /// </summary>
    bool MoveEntity(IEntity e, TileModel toTile, CourseIntent intent);

    /// <summary>
    ///     Stops the party for any actions.
    /// </summary>
    bool StopEntity(IEntity party);

    /// <summary>
    /// Starts to build a building
    /// </summary>
    bool Build(IEntity e, BuildingSpecId building, Location buildAtPosition);

    /// <summary>
    /// Ticks autoplay brain
    /// </summary>
    void TickAutoplay();
}

public class ActionsModule : IActionModule
{
	private readonly LisergySDK _client;
	private readonly AutoplayController _autoplay;
	public ActionsModule(LisergySDK client)
	{
		_client = client;
		_autoplay = new AutoplayController(client);
	}

	public void Register()
	{
	}

	public void TickAutoplay()
	{
		_autoplay.Tick();
	}

	public bool MoveEntity(IEntity entity, TileModel destinationTile, CourseIntent intent)
	{
		if (entity == null)
		{
			_client.SDKLog.Error("Error invalid entity");
			return false;
		}

		var entityId = entity.EntityId;
		if (entity.OwnerID != _client.Modules.Player.PlayerId)
		{
			_client.SDKLog.Error($"Cannot Move Entity {entityId} is not own entity");
			return false;
		}

		if (!entity.Components.TryGet<MapPlacementComponent>(out var placement))
		{
			_client.SDKLog.Error($"Cannot Move Entity {entityId} it is not placed in the map");
			return false;
		}

		var w = _client.Game.World;
		var sourceTile = w.GetTile(placement.Position.X, placement.Position.Y);
		if (!destinationTile.Logic.Vision.GetPlayersViewing().Any(p => p == entity.OwnerID))
		{
			_client.SDKLog.Error($"Cannot Move Entity {entityId} because target tile is not visible");
			return false;
		}

		var path = w.FindPath(sourceTile, destinationTile);
		if (path == null || path.Count() == 0)
		{
			_client.SDKLog.Error($"Cannot Move Entity {entityId} it is not placed in the map");
			return false;
		}

		foreach (var pathNode in path)
		{
			var tile = w.GetTile(pathNode.X, pathNode.Y);
			if (tile == null)
			{
				_client.SDKLog.Error($"Trying to walk path in {pathNode.X} {pathNode.Y} but tile was not yet received");
				return false;
			}

			var tileView = _client.Modules.Views.GetEntityView(tile.Entity);

			if (tileView == null || tileView.State == EntityViewState.NOT_RENDERED)
			{
				_client.SDKLog.Error($"Cannot Move Entity {entityId} by a path that is not known by the client");
				return false;
			}
		}

		_client.SDKLog.Debug($"Sending request to move party {entity} {path.Count()} tiles");
		_client.ClientEvents.Call(new EntityMovementRequestStarted
		{
			Destination = destinationTile,
			Path = path,
			Intent = intent,
			Party = entity
		});
		_client.Network.SendToServer(new MoveEntityCommand
		{
			Entity = entity.EntityId,
			Intent = intent,
			Path = path
		});
		return true;
	}

	public bool StopEntity(IEntity party)
	{
		_client.Network.SendToServer(new StopHarvestingCommand
		{
			EntityId = party.EntityId
		});
		return true;
	}

	public bool Build(IEntity e, BuildingSpecId building, Location buildAtPosition)
	{
		var w = _client.Game.World;
		var ePos = e.Get<MapPlacementComponent>().Position;
		var tile = w.GetTile(ePos.X, ePos.Y);
		var path = w.FindPath(tile, w.GetTile(buildAtPosition));
		if (path == null || path.Count() == 0)
		{
			_client.SDKLog.Error($"Cannot Move Entity {e} to build site");
			return false;
		}

		var brain = _autoplay.GetBrain(e);
		brain.Add(new BrainTask()
		{
			Command = new BuildCommand()
			{
				Location = buildAtPosition,
				Building = building,
			}, 
			IsComplete = () => tile.Logic.Tile.GetBuildingOnTile() != null,
		});
		brain.Add(new BrainTask()
		{
			Command = new MoveEntityCommand()
			{
				Path = path,
				Intent = CourseIntent.Defensive,
				Entity = e.EntityId
			},
			IsComplete = () => e.Get<MapPlacementComponent>().Position == ePos,
		});
		brain.Add(new BrainTask()
		{
			Command = new AssignBuilderCommand()
			{
				Location = buildAtPosition,
				EntityId = e.EntityId,
			}, 
			IsComplete = () => e.Components.Has<ConstructionWorkerComponent>(),
		});
		return true;//
	}
}