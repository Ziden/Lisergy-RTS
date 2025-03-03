using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Entities;
using System.Collections.Generic;

namespace ClientSDK.Data
{
    public class ViewContainer : DefaultValueDictionary<EntityType, Dictionary<GameId, IEntityView>>
    {
        public Dictionary<GameId, IEntityView> GetViews(EntityType t) => this[t];

        public void RemoveView(IEntityView view)
        {
            GetViews(view.Entity.EntityType).Remove(view.Entity.EntityId);
        }

        public IEntityView GetView(IEntity e)
        {
            if (e == null) return default!;
            if (GetViews(e.EntityType).TryGetValue(e.EntityId, out var v))
            {
                return v;
            }
            return default!;
        }

        public IEntityView GetView(EntityType t, GameId id)
        {
            if (GetViews(t).TryGetValue(id, out var v))
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
