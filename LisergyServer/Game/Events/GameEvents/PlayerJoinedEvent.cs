using Game.Systems.Player;

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
