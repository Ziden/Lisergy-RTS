using ClientSDK.Data;
using Game.Engine.ECLS;

namespace ClientSDK.Services
{
    public interface IGameView : IClientModule
    {
        /// <summary>
        /// Gets or creates a given entity view
        /// </summary>
        IEntityView GetOrCreateView(IEntity entity);

        /// <summary>
        /// Gets an uncasted entity view
        /// </summary>
        IEntityView GetEntityView(IEntity entity);
    }

    public class GameViewModule : IGameView
    {
        private GameClient _client;

        public ViewContainer _views = new ViewContainer();

        public GameViewModule(GameClient client)
        {
            _client = client;
        }

        public IEntityView GetOrCreateView(IEntity entity)
        {
            var existingView = _views.GetView(entity);
            if (existingView == null)
            {
                existingView = new EntityView(entity, _client);
                _views.AddView(entity, existingView);
                return existingView;
            }
            return existingView;
        }

        public void Register()
        {

        }

        public IEntityView GetEntityView(IEntity entity) => _views.GetView(entity);
    }
}
