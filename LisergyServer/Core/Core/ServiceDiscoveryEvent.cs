using Game.Events;
using System;

namespace BaseServer.Core
{
    [Serializable]
    public class ServiceDiscoveryEvent : ServerPacket
    {
        public string IP;
        public ushort Port;
        public ServerType ServerType;
    }
}
