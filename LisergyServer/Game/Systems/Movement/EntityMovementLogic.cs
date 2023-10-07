using Game.ECS;

namespace Game.Systems.Movement
{
    public class EntityMovementLogic : BaseEntityLogic<EntityMovementComponent>
    {
        public CourseTask GetCourse() => Entity.Components.Get<EntityMovementComponent>().Course;

        public void SetCourse(CourseTask newCourse)
        {
            var existingCourse = GetCourse();
            if (existingCourse != null && !existingCourse.HasFinished)
                existingCourse.Cancel();
            Entity.Components.Get<EntityMovementComponent>().Course = newCourse;
        }
    }
}
