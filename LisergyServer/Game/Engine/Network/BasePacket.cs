using Game.Engine.DataTypes;
using Game.Systems.Player;
using System;


namespace Game.Engine.Network
{
    public interface IPacket
    {
        public int ConnectionID { get; }

        public GameId SenderPlayerId { get; }
    }

    [Serializable]
    public class BasePacket : IPacket
    {
        [NonSerialized]
        private int _connectionId = 0;

        [NonSerialized]
        private GameId _senderPlayerId;

        [NonSerialized]
        public PlayerModel Sender;

        public GameId SenderPlayerId { get => _senderPlayerId; set => _senderPlayerId = value; }
        public int ConnectionID { get => _connectionId; set => _connectionId = value; }

    }
}
