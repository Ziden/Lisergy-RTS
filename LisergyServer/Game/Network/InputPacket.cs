using System;
using Game.Events;
using Game.Systems.Player;

namespace Game.Network
{
    /// <summary>
    /// Networking events sent from client
    /// </summary>
    [Serializable]
    public abstract class InputPacket : BasePacket
    {
        [NonSerialized]
        public PlayerEntity Sender;
    }
}
