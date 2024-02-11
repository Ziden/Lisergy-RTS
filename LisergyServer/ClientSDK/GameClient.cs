using ClientSDK.Data;
using Game;
using Game.Events.Bus;
using Game.Network;
using System;

namespace ClientSDK
{
    /// <summary>
    /// Main client SDK. Should be imported by the game client and consumed to run and display the game
    /// Contains all base functionality to run parts of the game client-side more easily.
    /// </summary>
    public interface IGameClient
    {
        /// <summary>
        /// Main game instance, where entities, networking and the world data are handled
        /// </summary>
        public IGame Game { get; }

        /// <summary>
        /// Client SDK modules that can perform server specific interactions
        /// </summary>
        public IServerModules Modules { get; }

        /// <summary>
        /// Exposed network to be used. References to Game.Network
        /// </summary>
        public IGameNetwork Network { get; }

        /// <summary>
        /// Client specific triggered event bus
        /// </summary>
        public EventBus<IClientEvent> ClientEvents { get; }

        /// <summary>
        /// General client SDK log
        /// </summary>
        public IGameLog Log { get; }
    }

    public class GameClient : IGameClient
    {
        public IGameNetwork Network { get; private set; }
        public IGame Game { get; private set; } = null!;
        public IServerModules Modules { get; private set; }
        public EventBus<IClientEvent> ClientEvents { get; private set; } = new EventBus<IClientEvent>();
        public IGameLog SDKLog { get; private set; }
        public IGameLog Log => Game.Log;

        public GameClient()
        {
            Serialization.LoadSerializers();
            SDKLog = new GameLog("[Client SDK]");
            Network = new ClientNetwork(SDKLog, ServerType.WORLD, ServerType.ACCOUNT, ServerType.CHAT);
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
