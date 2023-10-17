using ClientSDK.Data;
using Game.ECS;

namespace ClientSDK.SDKEvents
{
    /// <summary>
    /// Triggered when the client gets aware of a new entity.
    /// This is only called when the entity is created for the first time
    /// So if player sees an entity, this entity goes to the fog and comes back this is not called twice
    /// This is not applied to tiles or players
    /// </summary>
    public class ClientAwareOfEntityEvent : IClientEvent
    {
        public IEntity Entity;
        public IEntityView View;
    }
}
