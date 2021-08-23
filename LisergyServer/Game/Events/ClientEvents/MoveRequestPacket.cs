using Game.Movement;
using Game.World;
using System;
using System.Collections.Generic;

namespace Game.Events
{
    [Serializable]
    public class MoveRequestPacket : ClientEvent
    {
        public MovementIntent Intent;
        public byte PartyIndex;
        public List<Position> Path;
    }

}
