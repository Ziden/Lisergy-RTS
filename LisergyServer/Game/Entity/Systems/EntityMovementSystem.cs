
using Game.ECS;
using Game.Entity.Components;
using Game.Events.GameEvents;
using Game.Movement;

namespace Game.World.Systems
{
    public class EntityMovementSystem : GameSystem<EntityMovementComponent , WorldEntity>
    {
        internal override void OnComponentAdded(WorldEntity owner, EntityMovementComponent component, EntitySharedEventBus<WorldEntity> events)
        {
            //events.RegisterComponentEvent<TileVisibilityChangedEvent, TileVisibilityComponent>(OnTileVisibilityUpdated);
        }

        public static CourseTask GetCourse(WorldEntity entity) => entity.Components.Get<EntityMovementComponent>().Course;

        public static void SetCourse(WorldEntity entity, CourseTask newCourse)
        {
            var existingCourse = GetCourse(entity);
            if (existingCourse != null && !existingCourse.HasFinished)
                existingCourse.Cancel();
            entity.Components.Get<EntityMovementComponent>().Course = newCourse;
        }
    }
}
