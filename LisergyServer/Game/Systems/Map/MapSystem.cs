using Game.ECS;

namespace Game.Systems.Map
{
    public class MapSystem : LogicSystem<MapPlacementComponent, MapLogic>
    {
        public MapSystem(LisergyGame game) : base(game) { }

        public override void OnEnabled()
        {

        }
    }
}
