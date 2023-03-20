using System;

namespace Game.Events
{
    [Serializable]
    public class AuthPacket : ClientPacket
    {
        public string Login { get; set; }
        public string Password;
        public int SpecVersion;
    }
}
