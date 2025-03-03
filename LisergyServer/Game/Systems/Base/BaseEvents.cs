using Game.Engine.Events;

namespace Game.Systems.Base
{
    public class EntityCreatedEvent : IBaseEvent
    {
        public ushort SpecId;
    }
}
