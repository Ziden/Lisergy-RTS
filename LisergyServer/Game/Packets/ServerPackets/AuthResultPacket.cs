using Game.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class AuthResultPacket : ServerPacket
    {
        public bool Success;
        public GameId PlayerID;
    }
}
