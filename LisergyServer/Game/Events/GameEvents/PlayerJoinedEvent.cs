using Game.Player;

namespace Game.Events.GameEvents
{
    public class PlayerJoinedEvent : GameEvent
    {
        public PlayerEntity Player;

        public PlayerJoinedEvent(PlayerEntity p)
        {
            Player = p;
        }
    }
}
