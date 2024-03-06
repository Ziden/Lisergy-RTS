using Game;
using Game.Engine.DataTypes;
using Game.Engine.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSDK.Data
{
    public class ViewContainer : DefaultValueDictionary<EntityType, Dictionary<GameId, IEntityView>>
    {
        public Dictionary<GameId, IEntityView> GetViews(EntityType t) => this[t];

        public void RemoveView(IEntityView view)
        {
            GetViews(view.BaseEntity.EntityType).Remove(view.BaseEntity.EntityId);
        }

        public T GetView<T>(IEntity e) where T : IEntityView
        {
            if (e == null) return default!;
            if (GetViews(e.EntityType).TryGetValue(e.EntityId, out var v))
            {
                return (T)v;
            }
            return default(T)!;
        }

        public IEntityView GetView(IEntity e)
        {
            if (GetViews(e.EntityType).TryGetValue(e.EntityId, out var v))
            {
                return v;
            }
            return null!;
        }

        public T AddView<T>(IEntity e, T view) where T : IEntityView
        {
            GetViews(e.EntityType)[e.EntityId] = view;
            return view;
        }
    }

}
