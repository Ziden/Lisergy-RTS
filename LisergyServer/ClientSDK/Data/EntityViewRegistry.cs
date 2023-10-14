using Game;
using Game.ECS;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ClientSDK.Data
{
    /// <summary>
    /// Handles client registration of entity view types so the sdk knows the available view objects
    /// Will use them for instantiation
    /// </summary>
    public class EntityViewRegistry
    {
        private Dictionary<Type, Type> _viewTypes = new Dictionary<Type, Type>();
        private Dictionary<Type, Func<IEntityView>> _buildFunctions = new Dictionary<Type, Func<IEntityView>>();
        public void RegisterView<EntityType, ViewType>() where EntityType : IEntity where ViewType : EntityView<EntityType>
        {
            _viewTypes[typeof(EntityType)] = typeof(ViewType);
            Log.Debug($"Registered view {typeof(ViewType).Name} for entity {typeof(EntityType).Name}");
        }

        public IEntityView CreateView<EntityType>() where EntityType : IEntity
        {
            var viewType = _viewTypes[typeof(EntityType)];
            if(_buildFunctions.TryGetValue(viewType, out var function)) {
                return function();
            }
            var buildFunction = Expression.Lambda<Func<EntityView<EntityType>>>(Expression.New(viewType)).Compile();
            _buildFunctions[viewType] = buildFunction;
            return buildFunction();
        }

    }

}
