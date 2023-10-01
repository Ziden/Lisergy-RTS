using Game.ECS;
using Game.Events.GameEvents;

namespace Game.Systems.Dungeon
{
    public class DungeonSystem : GameSystem<DungeonComponent, DungeonEntity>
    {
        public override void OnEnabled()
        {
            SystemEvents.On<BattleFinishedEvent>(OnBattleFinish);
        }

        private static void OnBattleFinish(DungeonEntity e, DungeonComponent component, BattleFinishedEvent ev)
        {
            if (e.BattleGroupLogic.IsDestroyed)
            {
                e.Tile = null;
            }
        }
    }
}
