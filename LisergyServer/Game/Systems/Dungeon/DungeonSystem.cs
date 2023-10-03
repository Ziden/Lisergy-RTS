using Game.ECS;

namespace Game.Systems.Dungeon
{
    public class DungeonSystem : GameSystem<DungeonComponent>
    {
        public DungeonSystem(GameLogic game) : base(game) { }
    }
}
