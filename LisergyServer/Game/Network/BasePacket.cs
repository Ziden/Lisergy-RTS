using Game.Systems.Player;
using System;


namespace Game.Network
{
    [Serializable]
    public class BasePacket
    {
        [NonSerialized]
        public int ConnectionID;

        [NonSerialized]
        public PlayerEntity Sender;
    }
}
