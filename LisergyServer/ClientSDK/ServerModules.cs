using ClientSDK.Data;
using ClientSDK.Services;

namespace ClientSDK
{
    public interface IClientModule
    {
        void Register();
    }

    /// <summary>
    /// Handles server communication and event backs.
    /// </summary>
    public interface IServerModules : IGameService
    {
        public IAccountModule Account { get; }
        public IPlayerModule Player { get; }
        public IWorldModule World { get; }
        public IGameView Views { get; }
    }

    public class ServerModules : IServerModules
    {
        public IAccountModule Account { get; }
        public IPlayerModule Player { get; }
        public IWorldModule World { get; }
        public IGameView Views { get; }

        public ServerModules(GameClient client)
        {
            Account = new AccountModule(client);
            Player = new PlayerModule(client);
            World = new WorldModule(client);
            Views = new GameViewModule(client);
        }

        public void Register()
        {
            Account.Register();
            Player.Register();
            World.Register();
            Views.Register();
        }

        public void OnSceneLoaded() {  }
    }
}
