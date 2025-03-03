using ClientSDK.Data;
using Game;
using Game.Systems.Player;

namespace ClientSDK.SDKEvents
{
    /// <summary>
    /// Triggered when the client created the game instance and its ready to start receiving data from server
    /// This is triggered right after game specs are received and the game instance is setup
    /// </summary>
    public class GameStartedEvent : IClientEvent
    {
        public IGame Game;
        public PlayerModel LocalPlayer;

        public GameStartedEvent(IGame game, PlayerModel pl)
        {
            Game = game;
            LocalPlayer = pl;
        }

        public T Clone<T>()
        {
            return (T)this.MemberwiseClone();
        }

    }
}
