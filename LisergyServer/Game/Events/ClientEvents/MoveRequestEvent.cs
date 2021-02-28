using Game.World;
using System;
using System.Collections.Generic;
using ZeroFormatter;

namespace Game.Events
{
    [Serializable]
    public class MoveRequestEvent : ClientEvent
    {
        public override EventID GetID() => EventID.PARTY_REQUEST_MOVE;

        public byte PartyIndex;
        public List<Position> Path;
    }
}
