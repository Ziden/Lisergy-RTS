using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Systems.Movement;
using Game.Tile;
using Game.World;
using System.Collections.Generic;

namespace ClientSDK.SDKEvents
{
    /// <summary>
    /// Event triggered whenever a player attempts to move an entity and the request is sent to server.
    /// Server can still stop the movement, this is a client sided event.
    /// After this is sent the client should start to receive component updates regarding the movement.
    /// </summary>
    public class EntityMovementRequestStarted : IClientEvent
    {
        public IEntity Party;
        public IEnumerable<Location> Path;
        public TileModel Destination;
        public CourseIntent Intent;
    }
}
