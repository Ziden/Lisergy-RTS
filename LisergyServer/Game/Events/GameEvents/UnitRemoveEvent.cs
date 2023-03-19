namespace Game.Events.GameEvents
{
    /// <summary>
    /// When a unit is removed from an entity. 
    /// </summary>
    public class UnitRemovedEvent : GameEvent
    {
        public Unit[] Units;
        public WorldEntity Entity;

        public UnitRemovedEvent(WorldEntity entity, params Unit[] units)
        {
            Units = units;
            Entity = entity;
        }
    }
}
