using Game.Events;


namespace ClientSDK.SDKEvents
{
    /// <summary>
    /// Triggered when the client gets aware of a new entity
    /// This is not applied to tiles or players
    /// </summary>
    internal class ClientAwareOfEntityEvent : IGameEvent,  IBaseEvent
    {
    }
}
