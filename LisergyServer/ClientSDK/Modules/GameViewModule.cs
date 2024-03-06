using ClientSDK.Data;
using Game.Engine.ECS;
using System;

namespace ClientSDK.Services
{
    public interface IGameView : IClientModule
    {
        /// <summary>
        /// Gets or creates a given entity view
        /// </summary>
        IEntityView GetOrCreateView(IEntity entity);

        /// <summary>
        /// Gets an entity view and casts to the given type.
        /// Will throw if not existant or wrong type
        /// </summary>
        T GetView<T>(IEntity entity) where T : IEntityView;

        /// <summary>
        /// Gets an uncasted entity view
        /// </summary>
        IEntityView GetEntityView(IEntity entity) ;

        /// <summary>
        /// Registers a view type. Whenever the client receives an entity of the given type it will instantiate the view of that type.
        /// </summary>
        void RegisterView<EntityType, ViewType>() where EntityType : IEntity where ViewType : EntityView<EntityType>;
    }

    public class GameViewModule : IGameView
    {
        private GameClient _client;

        public ViewContainer _views = new ViewContainer();
        private EntityViewRegistry _viewRegistry = new EntityViewRegistry();

        public GameViewModule(GameClient client)
        {
            _client = client;
        }

        public void RegisterView<EntityType, ViewType>() where EntityType : IEntity where ViewType : EntityView<EntityType> {
            _viewRegistry.RegisterView<EntityType, ViewType>();
        }

        public IEntityView GetOrCreateView(IEntity entity)
        {
            var existingView = _views.GetView(entity);
            if (existingView == null)
            {
                existingView = _viewRegistry.CreateView(entity.GetType());
                _views.AddView(entity, existingView);
                var clientView = (IClientEntityView)existingView;
                clientView.Attach(_client, entity);
                return existingView;
            }
            return existingView;
        }

        public T GetView<T>(IEntity entity) where T : IEntityView => (T)_views.GetView(entity);

        public void Register()
        {
            
        }

        public IEntityView GetEntityView(IEntity entity) => _views.GetView(entity);
    }
}
