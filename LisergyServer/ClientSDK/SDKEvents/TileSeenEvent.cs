using ClientSDK.Data;
using Game.Engine.ECLS;

namespace ClientSDK.SDKEvents
{
    /// <summary>
    /// Fired when an entity is seen and not known by client
    /// </summary>
    public class TileSeenEvent(IEntity e) : IClientEvent
    {
        public IEntity Entity = e;
    }
}
