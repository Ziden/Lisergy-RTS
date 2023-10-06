using Game.ECS;

namespace Game.Systems.Map
{
    public class MapSystem : LogicSystem<MapPositionComponent, MapLogic>
    {
        public MapSystem(LisergyGame game) : base(game) { }

        public override void OnEnabled()
        {

        }
    }
}
