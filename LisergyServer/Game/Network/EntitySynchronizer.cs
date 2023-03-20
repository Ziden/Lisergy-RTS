using Game.ECS;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace Game.Network
{
    /// <summary>
    /// Because reflection can be expensive we cache where we can read logics from
    /// </summary>
    internal class LogicPropertyCache
    {
        public Type EntityType;
        public Type LogicSyncPropType;
        public Delegate Acessor;
    }

    /// <summary>
    /// Responsible for setting up basic generic data synchronization between client and server.
    /// Specific sync will be done inside views.
    /// </summary>
    public static class ComponentSynchronizer
    {
        /// <summary>
        /// Sync data of components received from server to client.
        /// Some components, specifically the ones who have logic objects, might have their unique way of sync.
        /// </summary>
        public static void SyncComponents(WorldEntity clientEntity, List<IComponent> syncedComponentsFromServer)
        {
            foreach (var serverComponent in syncedComponentsFromServer)
            {
                if (!clientEntity.Components.Has(serverComponent.GetType()))
                {
                    clientEntity.Components.Add(serverComponent);
                }

                if (TrySyncLogicalComponent(clientEntity, serverComponent))
                    return;

                serverComponent.CopyFieldsTo(clientEntity.Components.Get(serverComponent.GetType()));
                serverComponent.CopyPropertiesTo(clientEntity.Components.Get(serverComponent.GetType()));
            }
        }

        private static IComponentEntityLogic GetLogic(IEntity entity, Type syncType)
        {
            foreach (var prop in entity.GetType().GetProperties())
            {
                if (syncType.IsAssignableFrom(prop.PropertyType))
                    return prop.GetValue(entity) as IComponentEntityLogic;
            }
            return null;
        }

        private static bool TrySyncLogicalComponent(WorldEntity entity, IComponent serverComponent)
        {
            var syncType = serverComponent.GetType().GetCustomAttribute(typeof(SyncedComponent)) as SyncedComponent;
            if (syncType == null || syncType.LogicPropSyncType == null) return false;
            var logic = GetLogic(entity, syncType.LogicPropSyncType);
            if (syncType.LogicPropSyncType.IsAssignableFrom(logic.GetType()))
            {
                serverComponent.CopyPropertiesTo(logic);
                return true;
            }
            return false;
        }

        private static void CopyFieldsTo(this object fromObject, object toObject)
        {
            FieldInfo[] toObjectFields = toObject.GetType().GetFields();
            foreach (FieldInfo propTo in toObjectFields)
            {
                FieldInfo propFrom = fromObject.GetType().GetField(propTo.Name);
                if (propFrom != null)
                    propTo.SetValue(toObject, propFrom.GetValue(fromObject));
            }
        }

        private static void CopyPropertiesTo(this object fromObject, object toObject, Type propertyMapType)
        {
            PropertyInfo[] toObjectProperties = propertyMapType.GetProperties();
            foreach (PropertyInfo propTo in toObjectProperties)
            {
                PropertyInfo propFrom = propertyMapType.GetProperty(propTo.Name);
                if (propFrom != null && propFrom.CanWrite)
                    propTo.SetValue(toObject, propFrom.GetValue(fromObject, null), null);
            }
        }

        private static void CopyPropertiesTo(this object fromObject, object toObject)
        {
            PropertyInfo[] toObjectProperties = toObject.GetType().GetProperties();
            foreach (PropertyInfo propTo in toObjectProperties)
            {
                PropertyInfo propFrom = fromObject.GetType().GetProperty(propTo.Name);
                if (propFrom != null && propFrom.CanWrite)
                    propTo.SetValue(toObject, propFrom.GetValue(fromObject, null), null);
            }
        }

    }
}
