using Game.ECS;
using Game.Events.GameEvents;

namespace Game.Dungeon
{
    public class DungeonSystem : GameSystem<DungeonComponent, DungeonEntity>
    {
        internal override void OnComponentAdded(DungeonEntity owner, DungeonComponent component, EntityEventBus events)
        {
            events.RegisterComponentEvent<DungeonEntity, BattleFinishedEvent, DungeonComponent>(OnBattleFinish);
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
