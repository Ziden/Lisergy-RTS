using Game.ECS;
using Game.Systems.Battler;

namespace Game.Events.GameEvents
{
    /// <summary>
    /// When a unit is removed from an entity. 
    /// </summary>
    public class UnitRemovedEvent : GameEvent
    {
        public Unit[] UnitsRemoved;
        public IEntity Entity;
        public BattleGroupComponent Group;

        public UnitRemovedEvent(IEntity entity, in BattleGroupComponent group, params Unit[] units)
        {
            UnitsRemoved = units;
            Entity = entity;
            Group = group;
        }
    }
}
