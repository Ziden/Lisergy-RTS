namespace ClientSDK.Services
{
    public interface IEntityModule : IClientModule
    {
    }

    public class EntityModule : IEntityModule
    {
        private IGameClient _client;

        public EntityModule(IGameClient client)
        {
            _client = client;
        }

        public void Register()
        {
           
        }
    }
}
