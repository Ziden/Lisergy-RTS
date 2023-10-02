using Game.ECS;
using Game.Events.GameEvents;

namespace Game.Systems.Dungeon
{
    public class DungeonSystem : GameSystem<DungeonComponent>
    {
        public override void OnEnabled()
        {
            SystemEvents.On<BattleFinishedEvent>(OnBattleFinish);
        }

        private static void OnBattleFinish(IEntity e, DungeonComponent component, BattleFinishedEvent ev)
        {
            if (e is DungeonEntity d && d.BattleGroupLogic.IsDestroyed)
            {
                d.Tile = null;
            }
        }
    }
}
