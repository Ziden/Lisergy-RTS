using Game.Engine.Network;
using Game.Systems.Player;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class LoginResultPacket : BasePacket, IServerPacket
    {
        public string Token;
        public bool Success;
        public PlayerProfileComponent Profile;
        public TimeSpan TokenDuration;
    }
}
