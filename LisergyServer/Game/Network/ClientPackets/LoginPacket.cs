using System;

namespace Game.Network.ClientPackets
{
    [Serializable]
    public class LoginPacket : BasePacket, IClientPacket
    {
        public string Login { get; set; }
        public string Password;
        public int SpecVersion;
    }
}
