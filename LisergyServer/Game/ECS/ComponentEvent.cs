using Game.Events;

namespace Game.ECS
{
    /// <summary>
    /// Events to be fired for registered components in entities
    /// </summary>
    public class ComponentEvent<EntityType, EventType> : BaseEvent
    {
        public EventType Event { get; private set; }

        public ComponentEvent(EventType ev)
        {
            Event = ev;
        }
    }
}
