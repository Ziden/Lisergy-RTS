using Game.ECS;
using System.Collections.Generic;
using System.Reflection;


namespace Game.Packets
{
    /// <summary>
    /// Responsible for setting up basic generic data synchronization between client and server.
    /// Specific sync will be done inside views.
    /// </summary>
    public static class EntitySynchronizer
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
                serverComponent.CopyFieldsTo(clientEntity.Components.Get(serverComponent.GetType()));
                serverComponent.CopyPropertiesTo(clientEntity.Components.Get(serverComponent.GetType()));

            }
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
