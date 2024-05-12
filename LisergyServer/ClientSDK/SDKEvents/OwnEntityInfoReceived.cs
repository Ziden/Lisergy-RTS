using ClientSDK.Data;
using Game.Engine.ECS;
using Game.Systems.Party;
using Game.Tile;

namespace ClientSDK.SDKEvents
{
    /// <summary>
    /// Whenever the local player receives info of his own entities
    /// This event can be used to display own entity updates on the UI 
    /// When this event fires the entity should have all its components synced.
    /// </summary>
    public class OwnEntityInfoReceived<EntityType> : IClientEvent where EntityType : IEntity
    {
        public EntityType Entity;

        public OwnEntityInfoReceived(EntityType e)
        {
            Entity = e;
        }
    }
}
