using Game.Events;
using Game.Pathfinder;
using Game.Systems.Movement;
using Game.Systems.World;
using System;
using System.Collections.Generic;

namespace Game.Network.ClientPackets
{
    [Serializable]
    public class MoveRequestPacket : ClientPacket
    {
        public MovementIntent Intent;
        public byte PartyIndex;
        public List<MapPosition> Path;
    }

}
