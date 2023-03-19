using Game;
using Game.ECS;
using Game.Entity.Components;
using Game.Entity.Logic;
using Game.Events.GameEvents;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Assets.Code.Entity
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
                serverComponent.CopyShallowTo(clientEntity.Components.Get(serverComponent.GetType()));

                // TODO: Abstract in a nice way
                // each logic is kinda atttached to a component and might know how to update itself
                if (serverComponent is BattleGroupComponent serverBattleGroup)
                {
                    ((IBattleableEntity)clientEntity).BattleLogic.UpdateUnits(serverBattleGroup.FrontLine());
                }
            }
        }

        /// <summary>
        /// Hook to base events depending on the components and not on the view.
        /// Some components will have default events hooked in.
        /// Maybe there's a better place and nicer way of doing this, im isolating this here now.
        /// </summary>
        public static void HookBaseEvents(IEntity entity)
        {
            if (entity.Components.Has<EntityMovementComponent>())
            {
                entity.Components.RegisterExternalComponentEvents<EntityMovementComponent, EntityMoveInEvent>(MovementCallback);
            }
        }

        private static void MovementCallback(EntityMovementComponent c, EntityMoveInEvent ev)
        {
            var view = GameView.GetView(ev.Entity);
            if (view == null || !view.Instantiated)
            {
                return;
            }
            view.GameObject.transform.position = new UnityEngine.Vector3(ev.ToTile.X, 0.5f, ev.ToTile.Y);
        }

        private static void CopyShallowTo(this IComponent fromObject, IComponent toObject)
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
