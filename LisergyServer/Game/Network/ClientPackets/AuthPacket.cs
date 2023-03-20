using Game.Events;
using System;

namespace Game.Network.ClientPackets
{
    [Serializable]
    public class AuthPacket : ClientPacket
    {
        public string Login { get; set; }
        public string Password;
        public int SpecVersion;
    }
}
