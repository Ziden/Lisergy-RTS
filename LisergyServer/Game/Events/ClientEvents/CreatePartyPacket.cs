using System;

namespace Game.Events.ClientEvents
{
    [Serializable]
    public class CreatePartyPacket : ClientEvent
    {
        public Unit[] Units;
    }
}
