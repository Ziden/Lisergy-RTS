using Game.Engine.DataTypes;
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
        public GameId SenderPlayerId;

        [NonSerialized]
        public PlayerModel Sender;
    }
}
