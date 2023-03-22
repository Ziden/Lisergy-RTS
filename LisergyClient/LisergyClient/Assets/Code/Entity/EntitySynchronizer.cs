using Game;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Movement;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Code.Views;


namespace Assets.Code.Entity
{
    public static class BaseEntityEvents
    {
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

            if (view is IMovementLogic)
            {
                ((IMovementLogic)view).Move(ev);
                return;
            }

            view.GameObject.transform.position = new UnityEngine.Vector3(ev.ToTile.X, view.GameObject.transform.position.y, ev.ToTile.Y);
        }
    }
}