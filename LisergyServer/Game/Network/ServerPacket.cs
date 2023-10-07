using System;
using Game.Events;

namespace Game.Network
{
    /// <summary>
    /// Networking events sent from the server
    /// </summary>
    [Serializable]
    public abstract class ServerPacket : BasePacket
    {

    }
}
