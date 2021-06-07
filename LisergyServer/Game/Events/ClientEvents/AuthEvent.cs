using System;

namespace Game.Events
{
    [Serializable]
    public class AuthEvent : ClientEvent
    {
        public string Login { get; set; }
        public string Password;
        public int SpecVersion;
    }
}
