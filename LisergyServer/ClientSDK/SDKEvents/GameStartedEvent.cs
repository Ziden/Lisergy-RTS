using ClientSDK.Data;
using Game;
using Game.ECS;
using Game.Events;
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
        public PlayerEntity LocalPlayer;

        public GameStartedEvent(IGame game, PlayerEntity pl)
        {
            Game = game;
            LocalPlayer = pl;
        }

    }
}
