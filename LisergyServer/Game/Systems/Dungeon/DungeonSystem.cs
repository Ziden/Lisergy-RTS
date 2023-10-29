using Game.ECS;
using Game.Systems.BattleGroup;

namespace Game.Systems.Dungeon
{
    public class DungeonSystem : GameSystem<DungeonComponent>
    {
        public DungeonSystem(LisergyGame game) : base(game) { }

        public override void OnEnabled()
        {
            EntityEvents.On<GroupDeadEvent>(OnGroupDead);
        }

        /// <summary>
        /// Dungeons should disappear when destroyed
        /// </summary>
        private void OnGroupDead(IEntity e, GroupDeadEvent ev)
        {
            GameLogic.GetEntityLogic(e).Map.SetPosition(null);
        }
    }
}
