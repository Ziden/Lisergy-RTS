using Game.ECS;
using Game.Movement;
using Game.World.Systems;
using System;

namespace Game.Entity.Components
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
