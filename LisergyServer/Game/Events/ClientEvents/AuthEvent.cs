using System;
using ZeroFormatter;

namespace Game.Events
{
    [Serializable]
    public class AuthEvent : ClientEvent
    {
        public override EventID GetID() => EventID.AUTH;
        public string Login { get; set; }
        public string Password;
    }
}
