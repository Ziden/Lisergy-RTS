using Game.ECS;

namespace Game.Systems.Movement
{
    public class EntityMovementSystem : GameSystem<EntityMovementComponent>
    {
        public static CourseTask GetCourse(IEntity entity) => entity.Components.Get<EntityMovementComponent>().Course;

        public static void SetCourse(IEntity entity, CourseTask newCourse)
        {
            var existingCourse = GetCourse(entity);
            if (existingCourse != null && !existingCourse.HasFinished)
                existingCourse.Cancel();
            entity.Components.Get<EntityMovementComponent>().Course = newCourse;
        }
    }
}
