using Game.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.GameEvents
{
    public class PartyStatusUpdateEvent : GameEvent
    {
        public Party Party;

        public PartyStatusUpdateEvent(Party p)
        {
            this.Party = p;
        }

    }
}
