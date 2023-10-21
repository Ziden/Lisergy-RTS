using ClientSDK.Data;
using Game.Network.ClientPackets;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSDK.SDKEvents
{
    /// <summary>
    /// Called when chat updates
    /// </summary>
    public class ChatUpdateEvent : IClientEvent
    {
        /// <summary>
        /// Can be null when receiving more than one
        /// </summary>
        public ChatPacket? NewPacket;
    }
}
