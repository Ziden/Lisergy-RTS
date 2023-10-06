using Game.ECS;
using Game.Systems.Battler;
using Game.Tile;

namespace Game.Events.GameEvents
{
    /// <summary>
    /// Fired when a entity with BattleGroupComponent finishes a battle and dies
    /// </summary>
    public class GroupDeadEvent : GameEvent
    {
        public IEntity Entity;
        public BattleGroupComponent GroupComponent;
    }
}




