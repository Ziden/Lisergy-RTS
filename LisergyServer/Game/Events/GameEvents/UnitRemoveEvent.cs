using Game.ECS;
using Game.Systems.Battler;

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
