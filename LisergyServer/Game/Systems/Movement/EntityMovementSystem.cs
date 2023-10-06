using Game.ECS;

namespace Game.Systems.Movement
{
    public class EntityMovementSystem : LogicSystem<EntityMovementComponent, EntityMovementLogic>
    {
        public EntityMovementSystem(LisergyGame game) : base(game) { }
    }
}
