using Game.Party;

namespace Game.Events.GameEvents
{
    public class PartyStatusUpdateEvent : GameEvent
    {
        public PartyEntity Party;

        public PartyStatusUpdateEvent(PartyEntity p)
        {
            Party = p;
        }

    }
}
