
using Game.DataTypes;
using Game.ECS;
using System;
using System.Collections.Generic;

namespace Assets.Code.Views
{
    public class ViewController
    {
        public DefaultValueDictionary<Type, Dictionary<GameId, IEntityView>> _views = new DefaultValueDictionary<Type, Dictionary<GameId, IEntityView>>();

        public Dictionary<GameId, IEntityView> GetViews(Type t) => _views[t];

        public void RemoveView(IEntityView view)
        {
            GetViews(view.GetType()).Remove(view.Entity.EntityId);
        }

        public T GetView<T>(IEntity e) where T : IEntityView
        {
            if (GetViews(e.GetType()).TryGetValue(e.EntityId, out var v))
            {
                return (T)v;
            }
            return default(T);
        }

        public IEntityView GetView(IEntity e)
        {
            if (GetViews(e.GetType()).TryGetValue(e.EntityId, out var v))
            {
                return v;
            }
            return null;
        }

        public T AddView<T>(IEntity e, T view) where T : IEntityView
        {
            GetViews(e.GetType())[e.EntityId] = view;
            return view;
        }
    }
}
