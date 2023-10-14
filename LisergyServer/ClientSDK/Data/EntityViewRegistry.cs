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
            var buildFunction = Expression.Lambda<Func<EntityView<EntityType>>>(Expression.New(typeof(ViewType))).Compile();
            _buildFunctions[typeof(EntityType)] = buildFunction;
            _viewTypes[typeof(EntityType)] = typeof(ViewType);
            Log.Debug($"Registered view {typeof(ViewType).Name} for entity {typeof(EntityType).Name}");
        }

        public IEntityView CreateView(Type entityType)
        {
            if (!_buildFunctions.TryGetValue(entityType, out var func)) 
                throw new SDKMissconfiguredException($"Entity type {entityType.Name} was not registered to have a game view");
            return func();
        }
    }

}
