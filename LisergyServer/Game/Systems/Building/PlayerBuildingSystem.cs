using Game.ECS;

namespace Game.Systems.Building
{
    public class PlayerBuildingSystem : GameSystem<PlayerBuildingComponent>
    {
        public PlayerBuildingSystem(LisergyGame game) : base(game) { }
    }
}
