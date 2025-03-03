using Game.Engine.ECLS;

namespace Game.Systems.Movement
{
    public class MovementSystem : LogicSystem<MovementComponent, MovementLogic>
    {
        public MovementSystem(LisergyGame game) : base(game) { }

        public override void RegisterListeners()
        {

        }
    }
}
