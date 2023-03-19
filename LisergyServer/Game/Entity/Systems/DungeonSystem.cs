using Game.ECS;
using Game.Entity.Components;
using Game.Entity.Entities;
using Game.Events.GameEvents;
using Game.World.Components;

namespace Game.Entity.Systems
{
    public class DungeonSystem : GameSystem<DungeonComponent, DungeonEntity>
    {
        internal override void OnComponentAdded(DungeonEntity owner, DungeonComponent component, EntityEventBus events)
        {
            events.RegisterComponentEvent<DungeonEntity, BattleFinishedEvent, DungeonComponent>(OnBattleFinish);
        }

        private static void OnBattleFinish(DungeonEntity e, DungeonComponent component, BattleFinishedEvent ev) 
        {
            if(e.BattleLogic.IsDestroyed)
            {
                e.Tile = null;
            }
        }
    }
}
