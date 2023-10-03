using Game.ECS;
using Game.Systems.Battler;

namespace Game.Events.GameEvents
{
    /// <summary>
    /// When a unit is added to an entity. 
    /// </summary>
    public class UnitAddToGroupEvent : GameEvent
    {
        public Unit[] Units;
        public IEntity Entity;
        public BattleGroupComponent Component;

        public UnitAddToGroupEvent(IEntity entity, BattleGroupComponent group, params Unit[] units)
        {
            Component = group;
            Units = units;
            Entity = entity;
        }
    }
}
