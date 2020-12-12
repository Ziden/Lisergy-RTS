using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class SeeChunkEvent : ServerEvent
    {
        public override EventID GetID() => EventID.SEE_CHUNK;
    }
}
