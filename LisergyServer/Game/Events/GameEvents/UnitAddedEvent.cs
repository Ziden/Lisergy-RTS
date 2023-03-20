using Game.Battler;
using Game.ECS;

namespace Game.Events.GameEvents
{
    /// <summary>
    /// When a unit is added to an entity. 
    /// </summary>
    public class UnitAddedEvent : GameEvent
    {
        public Unit[] Units;
        public IEntity Entity;

        public UnitAddedEvent(IEntity entity, params Unit[] units)
        {
            Units = units;
            Entity = entity;
        }
    }
}
