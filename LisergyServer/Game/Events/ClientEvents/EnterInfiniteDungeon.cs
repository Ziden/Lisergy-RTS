using System;

namespace Game.Events.ClientEvents
{
    [Serializable]
    public class EnterInfiniteDungeon : ClientEvent
    {
        public string PartyID;
    }
}
