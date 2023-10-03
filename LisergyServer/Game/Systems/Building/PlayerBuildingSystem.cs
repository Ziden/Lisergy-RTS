using Game.ECS;
using Game.Events.GameEvents;

namespace Game.Systems.Building
{
    public class PlayerBuildingSystem : GameSystem<PlayerBuildingComponent>
    {
        public PlayerBuildingSystem(GameLogic game) : base(game) { }
    }
}
