using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Entities;
using System;
using System.Collections.Generic;

namespace ClientSDK.Services
{
    public class GameViewModule(LisergySDK client)
    {
        public event Action<EntityView>? OnViewCreated;

        private Dictionary<EntityType, Func<IEntity, EntityView>> _creators = new();

        public ViewContainer _views = new ViewContainer();

        public void RegisterView(EntityType t, Func<IEntity, EntityView> creator)
        {
            _creators[t] = creator; 
        }

        public IEntityView GetOrCreateView(IEntity entity)
        {
            var existingView = _views.GetView(entity);
            if (existingView == null)
            {
                EntityView v;
                if (_creators.TryGetValue(entity.EntityType, out var creator))
                {
                    v = creator(entity);

                } else
                {
                    v = new EntityView(entity, client);
                }
                _views.AddView(entity, v);
                OnViewCreated?.Invoke(v);
                return v;
            }
            return existingView;
        }

        public void Register()
        {

        }

        public IEntityView GetEntityView(IEntity entity) => _views.GetView(entity);
    }
}
