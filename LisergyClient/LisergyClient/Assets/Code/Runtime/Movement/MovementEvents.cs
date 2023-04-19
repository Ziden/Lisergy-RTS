using Game;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Movement;
using Assets.Code.Views;
using Assets.Code.Assets.Code.Runtime.Movement;
using UnityEngine;

namespace Assets.Code.Entity
{
    public static class MovementEvents
    {
        /// <summary>
        /// Hook to base events depending on the components and not on the view.
        /// Some components will have default events hooked in.
        /// Maybe there's a better place and nicer way of doing this, im isolating this here now.
        /// </summary>
        public static void HookMovementEvents(IEntity entity)
        {
            if (entity.Components.Has<EntityMovementComponent>())
            {
                entity.Components.RegisterExternalComponentEvents<EntityMovementComponent, EntityMoveInEvent>(MovementCallback);
            }
        }

        private static void MovementCallback(EntityMovementComponent c, EntityMoveInEvent ev)
        {
            var view = GameView.GetView(ev.Entity);
            if (view == null || !view.NeedsInstantiate)
            {
                return;
            }

            if (!view.Entity.Components.Has<ClientMovementInterpolationComponent>())
            {
                view.GameObject.transform.position = new Vector3(ev.ToTile.X, view.GameObject.transform.position.y, ev.ToTile.Y);
            }
        }
    }
}