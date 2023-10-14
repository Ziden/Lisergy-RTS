using ClientSDK.Data;
using ClientSDK.Modules;
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
        public IEntityModule Entities { get; }
        public IComponentsModule Components { get; }
    }

    public class ServerModules : IServerModules
    {
        public IAccountModule Account { get; }
        public IPlayerModule Player { get; }
        public IWorldModule World { get; }
        public IGameView Views { get; }
        public IEntityModule Entities { get; }

        public IComponentsModule Components { get; }

        public ServerModules(GameClient client)
        {
            Account = new AccountModule(client);
            Player = new PlayerModule(client);
            World = new WorldModule(client);
            Views = new GameViewModule(client);
            Entities = new EntityModule(client);
            Components = new ComponentsModule();
        }

        public void Register()
        {
            Account.Register();
            Player.Register();
            World.Register();
            Views.Register();
            Entities.Register();
            Components.Register();
        }

        public void OnSceneLoaded() {  }
    }
}
