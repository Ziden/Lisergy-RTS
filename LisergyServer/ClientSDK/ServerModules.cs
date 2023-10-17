using ClientSDK.Data;
using ClientSDK.Modules;
using ClientSDK.Services;

namespace ClientSDK
{
    /// <summary>
    /// A client SDK module that helps to integrate with the game and server data
    /// </summary>
    public interface IClientModule
    {
        /// <summary>
        /// Registers the module after all modules are initialized
        /// </summary>
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
        public IActionModule Actions { get; }
    }

    public class ServerModules : IServerModules
    {
        public IAccountModule Account { get; }
        public IPlayerModule Player { get; }
        public IWorldModule World { get; }
        public IGameView Views { get; }
        public IEntityModule Entities { get; }
        public IComponentsModule Components { get; }
        private ILogicModule Logic { get; }
        public IActionModule Actions { get; }

        public ServerModules(GameClient client)
        {
            Account = new AccountModule(client);
            Player = new PlayerModule(client);
            World = new WorldModule(client);
            Views = new GameViewModule(client);
            Entities = new EntityModule(client);
            Components = new ComponentsModule();
            Logic = new LogicModule(client);
            Actions = new ActionsModule(client);
        }

        public void Register()
        {
            Account.Register();
            Player.Register();
            World.Register();
            Views.Register();
            Entities.Register();
            Components.Register();
            Logic.Register();
            Actions.Register();
        }

        public void OnSceneLoaded() {  }
    }
}
