using Game.ECS;

namespace Game.Systems.Movement
{
    public class CourseSystem : LogicSystem<CourseComponent, CourseLogic>
    {
        public CourseSystem(LisergyGame game) : base(game) { }

        public override void RegisterListeners()
        {
            
        }
    }
}
