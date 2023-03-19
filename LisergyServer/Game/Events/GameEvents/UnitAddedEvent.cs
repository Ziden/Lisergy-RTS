namespace Game.Events.GameEvents
{
    /// <summary>
    /// When a unit is added to an entity. 
    /// </summary>
    public class UnitAddedEvent : GameEvent
    {
        public Unit[] Units;
        public WorldEntity Entity;

        public UnitAddedEvent(WorldEntity entity, params Unit[] units)
        {
            Units = units;
            Entity = entity;
        }
    }
}
