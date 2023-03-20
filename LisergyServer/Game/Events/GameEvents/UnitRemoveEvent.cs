using Game.Battler;
using Game.ECS;

namespace Game.Events.GameEvents
{
    /// <summary>
    /// When a unit is removed from an entity. 
    /// </summary>
    public class UnitRemovedEvent : GameEvent
    {
        public Unit[] Units;
        public IEntity Entity;

        public UnitRemovedEvent(IEntity entity, params Unit[] units)
        {
            Units = units;
            Entity = entity;
        }
    }
}
