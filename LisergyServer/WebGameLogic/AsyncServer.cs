using Game;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.Network;
using Game.Events.ServerEvents;
using Game.Services;
using WebPlayerLogic.Playfab;

namespace MapServer
{
    /// <summary>
    /// Unified one server for everything, for now
    /// </summary>
    public class AsyncServer
    {
        private string _playerId;
        public LisergyGame Game { get; private set; }
        private GameServerNetwork _network;
        private BattleService _battleService;
        private WorldService _worldService;
        private EntityPersistence _persistence;

        public AsyncServer(string playerId, LisergyGame game) : base()
        {
            Serialization.LoadSerializers();
            Game = game;
            _playerId = playerId;
            _network = game.Network as GameServerNetwork;
            _battleService = new BattleService(Game);
            _worldService = new WorldService(Game);
            _persistence = new EntityPersistence(game);
            _network.OnOutgoingPacket += SendPacketToPlayer;
        }

        protected void SendPacketToPlayer(GameId player, BasePacket packet)
        {
            if (packet is EntityUpdatePacket e)
            {
                var entity = Game.Entities[e.EntityId];
                _ = _persistence.SaveEntity(_playerId, entity);
            }
        }
    }
}
