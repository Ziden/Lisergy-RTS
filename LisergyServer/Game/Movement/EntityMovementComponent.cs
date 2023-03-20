using Game.ECS;
using System;

namespace Game.Movement
{
    public class EntityMovementComponent : IComponent
    {
        public CourseTask Course;

        public TimeSpan MoveDelay;

        static EntityMovementComponent()
        {
            SystemRegistry<EntityMovementComponent, WorldEntity>.AddSystem(new EntityMovementSystem());
        }
    }
}
