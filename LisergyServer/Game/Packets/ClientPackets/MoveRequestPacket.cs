using Game.Engine.Network;
using Game.Systems.Movement;
using Game.World;
using System;
using System.Collections.Generic;

namespace Game.Network.ClientPackets
{
    [Serializable]
    public class MoveRequestPacket : BasePacket, IClientPacket
    {
        public CourseIntent Intent;
        public byte PartyIndex;
        public List<Location> Path;
    }

}
