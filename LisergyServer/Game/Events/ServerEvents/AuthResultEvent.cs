using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class AuthResultEvent : ServerEvent
    {
        public override EventID GetID() => EventID.AUTH_RESULT;

        public GameConfiguration Configuration;

        public bool Success;
        public string PlayerID;
    }
}
