using Game.Systems.Player;
using System;


namespace Game.Engine.Network
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
