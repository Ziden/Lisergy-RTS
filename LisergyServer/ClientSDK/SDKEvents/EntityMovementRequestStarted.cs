using System.Collections.Generic;
using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Systems.Movement;
using Game.Tile;
using Game.World;

namespace ClientSDK.SDKEvents;

/// <summary>
///     Event triggered whenever a player attempts to move an entity and the request is sent to server.
///     Server can still stop the movement, this is a client sided event.
///     After this is sent the client should start to receive component updates regarding the movement.
/// </summary>
public class EntityMovementRequestStarted : IClientEvent
{
	public required TileModel Destination;
	public CourseIntent Intent;
	public required IEntity Party;
	public required IEnumerable<Location> Path;
}