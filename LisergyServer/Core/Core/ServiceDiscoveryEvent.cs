using LisergyServer.Core;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class ServiceDiscoveryEvent : ServerPacket
    {
        public string IP;
        public ushort Port;
        public ServerType ServerType;
    }
}
