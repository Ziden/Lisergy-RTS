using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class AuthResultEvent : ServerEvent
    {
        public bool Success;
        public string PlayerID;
    }
}
