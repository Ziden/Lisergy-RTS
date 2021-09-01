using Game.Entity;
using GameData;
using System;
using System.Linq;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class PartyUpdatePacket : ServerEvent
    {
        public Party Party;

        public PartyUpdatePacket(Party party)
        {
            Party = party;
        }
    }
}
