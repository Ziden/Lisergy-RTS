using Game.Engine.ECLS;

namespace Game.Engine.Events
{
    public class ComponentUpdateEvent<T> : IBaseEvent
    {
        public IEntity Entity;
        public T Old;
        public T New;
    }
}
