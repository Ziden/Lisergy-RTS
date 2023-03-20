using Game.Events;
using Game.Movement;
using Game.Pathfinder;
using System;
using System.Collections.Generic;

namespace Game.Network.ClientPackets
{
    [Serializable]
    public class MoveRequestPacket : ClientPacket
    {
        public MovementIntent Intent;
        public byte PartyIndex;
        public List<Position> Path;
    }

}
