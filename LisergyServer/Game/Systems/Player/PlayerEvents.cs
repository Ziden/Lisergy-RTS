using Game.Engine.Events;

namespace Game.Systems.Player
{
    /// <summary>
    /// Called when a player joins (connects) to the world
    /// </summary>
    public class PlayerJoinedEvent : IGameEvent
    {
        public PlayerModel Player;

        public PlayerJoinedEvent(PlayerModel p)
        {
            Player = p;
        }
    }
}
