using Game.ECS;


namespace Game.Systems.Player
{
    public class PlayerSystem : LogicSystem<PlayerDataComponent, PlayerLogic>
    {
        public PlayerSystem(LisergyGame game) : base(game)
        {
        }
    }
}
