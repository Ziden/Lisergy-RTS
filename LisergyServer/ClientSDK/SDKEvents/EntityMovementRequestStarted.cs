using ClientSDK.Data;
using Game.Pathfinder;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Tile;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSDK.SDKEvents
{
    /// <summary>
    /// Event triggered whenever a player attempts to move an entity and the request is sent to server.
    /// Server can still stop the movement, this is a client sided event.
    /// After this is sent the client should start to receive component updates regarding the movement.
    /// </summary>
    public class EntityMovementRequestStarted : IClientEvent
    {
        public PartyEntity Party;
        public List<PathFinderNode> Path;
        public TileEntity Destination;
        public CourseIntent Intent;
    }
}
