using Game.ECS;
using System;

namespace Game.Systems.Movement
{
    public class EntityMovementComponent : IComponent
    {
        public CourseTask Course;
        public TimeSpan MoveDelay;
    }
}
