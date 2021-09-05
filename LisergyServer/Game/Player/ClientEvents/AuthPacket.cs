using System;

namespace Game.Events
{
    [Serializable]
    public class AuthPacket : ClientEvent
    {
        public string Login { get; set; }
        public string Password;
        public int SpecVersion;
    }
}
