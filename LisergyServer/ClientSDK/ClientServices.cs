using ClientSDK.Services;

namespace ClientSDK
{
    public interface IClientService
    {
        void Register();
    }

    public interface IClientServices
    {
        public IAccountService Account { get; }
        public IPlayerService Player { get; }
        public IWorldService World { get; }
    }

    public class ClientServices : IClientServices
    {
        public IAccountService Account { get; }
        public IPlayerService Player { get; }
        public IWorldService World { get; }

        public ClientServices(GameClient client)
        {
            Account = new AccountService(client);
            Player = new PlayerService(client);
            World = new WorldService(client);
        }

        public void Register()
        {
            Account.Register();
            Player.Register();
            World.Register();
        }
    }
}
