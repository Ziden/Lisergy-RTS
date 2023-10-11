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
        public IClientServices Services { get; }
    }

    public class GameClient : IGameClient
    {
        public IGameNetwork ClientNetwork { get; private set; } = new ClientNetwork();
        public IGame Game { get; private set; }    
        public IClientServices Services { get; private set; }

        public GameClient() 
        {
            Serialization.LoadSerializers();
            var s = new ClientServices(this);
            Services = s;
            s.Register();
        }
    }
}
