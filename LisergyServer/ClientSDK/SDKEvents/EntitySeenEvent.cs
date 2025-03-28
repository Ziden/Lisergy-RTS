using ClientSDK.Data;
using Game.Engine.ECLS;

namespace ClientSDK.SDKEvents
{
    /// <summary>
    /// Fired when an entity is seen and not known by client
    /// </summary>
    public class EntitySeenEvent : IClientEvent
    {
        public IEntity Entity;

        public EntitySeenEvent(IEntity e)
        {
            Entity = e;
        }
    }
}
