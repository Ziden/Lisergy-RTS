using Game.DataTypes;
using Game.Events.Bus;
using Game.Network;
using Game.Systems.Player;
using Game.World;
using System;
using System.Linq;

namespace Game
{
    public enum ServerType
    {
        BATTLE, WORLD
    }

    public interface IGameNetwork
    {
        void ReceiveInput(GameId sender, InputPacket input);
        public void On<T>(Action<T> listener) where T : BasePacket;
        public void SendToPlayer(ServerPacket p, params PlayerEntity[] players);
        public void SendToServer(ServerPacket p, ServerType server);
    }

    public class GameNetwork : IGameNetwork, IEventListener
    {
        public Action<GameId, ServerPacket> OnOutgoingPacket;

        public EventBus<BasePacket> IncomingPackets { get; } = new EventBus<BasePacket>();

        private IGame _game;

        public GameNetwork(IGame game)
        {
            _game = game;
        }

        public void On<T>(Action<T> listener) where T : BasePacket
        {
            IncomingPackets.Register(this, listener);
        }

        public void ReceiveInput(GameId sender, InputPacket input)
        {
            input.Sender = _game.Players[sender] ?? new PlayerEntity(_game);
            IncomingPackets.Call(input);
            _game.Entities.DeltaCompression.SendDeltaPackets(input.Sender);
        }

        public void SendToServer(ServerPacket p, ServerType server)
        {
            IncomingPackets.Call(p); // Hack for local server for now
        }

        public void SendToPlayer(ServerPacket p, params PlayerEntity[] players)
        {
            foreach (var player in players)
            {
                if (player == null) continue;
                OnOutgoingPacket?.Invoke(player.EntityId, p);
            }
        }
    }
}
