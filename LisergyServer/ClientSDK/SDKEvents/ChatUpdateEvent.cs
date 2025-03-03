using ClientSDK.Data;
using Game.Network.ClientPackets;

namespace ClientSDK.SDKEvents
{
    /// <summary>
    /// Called when chat updates. Either when receiving a new message or chat log received.
    /// </summary>
    public class ChatUpdateEvent : IClientEvent
    {
        /// <summary>
        /// Can be null when receiving more than one
        /// </summary>
        public ChatPacket? NewPacket;
    }
}
