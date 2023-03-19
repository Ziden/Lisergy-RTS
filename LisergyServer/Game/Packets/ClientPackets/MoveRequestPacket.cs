using Game.Movement;
using Game.World;
using System;
using System.Collections.Generic;

namespace Game.Events
{
    [Serializable]
    public class MoveRequestPacket : ClientPacket
    {
        public MovementIntent Intent;
        public byte PartyIndex;
        public List<Position> Path;
    }

}
