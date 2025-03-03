using ClientSDK.Data;
using Game.Engine.ECLS;

namespace ClientSDK.SDKEvents
{
    /// <summary>
    /// Whenever the local player receives info of his own entities
    /// This event can be used to display own entity updates on the UI 
    /// When this event fires the entity should have all its components synced.
    /// </summary>
    public class OwnEntityInfoReceived : IClientEvent
    {
        public IEntity Entity;

        public OwnEntityInfoReceived(IEntity e)
        {
            Entity = e;
        }
    }
}
