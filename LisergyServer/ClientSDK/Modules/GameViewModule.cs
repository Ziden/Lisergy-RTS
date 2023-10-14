using ClientSDK.Data;
using Game.ECS;

namespace ClientSDK.Services
{
    public interface IGameView : IClientModule
    {
        /// <summary>
        /// Gets or creates a given entity view
        /// </summary>
        IEntityView GetOrCreate<T>(IEntity entity) where T : IEntity;

        /// <summary>
        /// Gets or creates a view for a given entity typed by call
        /// </summary>
        IEntityView GetOrCreateView<T>(T entity) where T : IEntity;

        /// <summary>
        /// Gets an entity view and casts to the given type.
        /// Will throw if not existant or wrong type
        /// </summary>
        T GetView<T>(IEntity entity) where T : IEntityView;

        /// <summary>
        /// Registers a view type. Whenever the client receives an entity of the given type it will instantiate the view of that type.
        /// </summary>
        void RegisterView<EntityType, ViewType>() where EntityType : IEntity where ViewType : EntityView<EntityType>;
    }

    public class GameViewModule : IGameView
    {
        private IGameClient _client;

        public ViewContainer _views = new ViewContainer();
        private EntityViewRegistry _viewRegistry = new EntityViewRegistry();

        public GameViewModule(IGameClient client)
        {
            _client = client;
        }

        public void RegisterView<EntityType, ViewType>() where EntityType : IEntity where ViewType : EntityView<EntityType> {
            _viewRegistry.RegisterView<EntityType, ViewType>();
        }

        public IEntityView GetOrCreateView<EntityType>(EntityType entity) where EntityType : IEntity
        {
            return GetOrCreate<EntityType>(entity);
        }

        public IEntityView GetOrCreate<EntityType>(IEntity entity) where EntityType : IEntity
        {
            var existingView = _views.GetView(entity);
            if (existingView == null)
            {
                existingView = _viewRegistry.CreateView<EntityType>();
                _views.AddView(entity, existingView);
                var casted = (EntityView<EntityType>)existingView;
                casted.Entity = (EntityType)entity;
                casted.Client = _client;
                return casted;
            }
            return existingView;
        }

        public T GetView<T>(IEntity entity) where T : IEntityView => (T)_views.GetView(entity);

        public void Register()
        {
            
        }
    }
}
