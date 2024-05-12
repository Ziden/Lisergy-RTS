using Game.Engine.ECS;

namespace Game.Systems.Construction
{
    public class ConstructionSystem : LogicSystem<ConstructionComponent, ConstructionLogic>
    {
        public ConstructionSystem(LisergyGame game) : base(game) { }

        public override void RegisterListeners()
        {

        }
    }
}
