using Game.DataTypes;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class AuthResultPacket : ServerPacket
    {
        public bool Success;
        public GameId PlayerID;
    }
}
