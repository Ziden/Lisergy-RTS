using Game.Entity;
using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class PartyVisibleEvent : ServerEvent
    {
        public Party Party;

        [NonSerialized]
        public WorldEntity Viewer;
     
        public override EventID GetID() => EventID.PARTY_VISIBLE;
    }
}
