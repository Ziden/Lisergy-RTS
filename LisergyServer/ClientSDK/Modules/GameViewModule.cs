using ClientSDK.Data;
using Game.Engine.ECLS;
using System;

namespace ClientSDK.Services
{
    public class GameViewModule
    {
        private GameClient _client;

        public ViewContainer _views = new ViewContainer();
        public Func<IEntity, EntityView> CreatorFunction;

        public GameViewModule(GameClient client)
        {
            _client = client;
        }

        public IEntityView GetOrCreateView(IEntity entity)
        {
            var existingView = _views.GetView(entity);
            if (existingView == null)
            {
                if (CreatorFunction == null)
                {
                    existingView = new EntityView(entity, _client);
                }
                else
                {
                    existingView = CreatorFunction(entity);
                }
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
