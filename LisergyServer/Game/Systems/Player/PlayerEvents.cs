using Game.Events;

namespace Game.Systems.Player
{
    /// <summary>
    /// Called when a player joins (connects) to the world
    /// </summary>
    public class PlayerJoinedEvent : IGameEvent
    {
        public PlayerEntity Player;

        public PlayerJoinedEvent(PlayerEntity p)
        {
            Player = p;
        }
    }
}
