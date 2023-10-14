using Game;
using Game.Network;
using System;

namespace ClientSDK
{
    /// <summary>
    /// Main client SDK. Should be imported by the game client and consumed
    /// to run and display the game
    /// </summary>
    public interface IGameClient 
    {
        public IGame Game { get; }
        public IServerModules Modules { get; }
        public IGameNetwork Network { get; }
    }

    public class GameClient : IGameClient
    {
        public IGameNetwork Network { get; private set; } = new ClientNetwork();
        public IGame Game { get; private set; }    
        public IServerModules Modules { get; private set; }

        public GameClient()
        {
            Serialization.LoadSerializers();
            var s = new ServerModules(this);
            Modules = s;
            s.Register();
        }

        public void InitializeGame(LisergyGame game)
        {
            Game = game;
        }
    }
}
