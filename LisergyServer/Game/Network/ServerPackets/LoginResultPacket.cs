using Game.DataTypes;
using Game.Network;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class LoginResultPacket : BasePacket, IServerPacket
    {
        public string Token;
        public bool Success;
        public GameId PlayerID;
    }
}
