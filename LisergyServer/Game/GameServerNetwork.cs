﻿using Game.DataTypes;
using Game.Events.Bus;
using Game.Network;
using Game.Systems.Player;
using Game.World;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Game
{
    public enum ServerType
    {
        BATTLE, WORLD
    }

    public interface IGameNetwork
    {
        /// <summary>
        /// Receive and proccess an input packet
        /// </summary>
        void ReceiveInput(GameId sender, BasePacket input);

        /// <summary>
        /// Listens for packets being received
        /// </summary>
        public void On<T>(Action<T> listener) where T : BasePacket;

        /// <summary>
        /// Sends a packet to player
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendToPlayer(BasePacket p, params PlayerEntity[] players);

        /// <summary>
        /// Sends a packet to server.
        /// </summary>
        public void SendToServer(BasePacket p, ServerType server = ServerType.WORLD);
    }

    public class GameServerNetwork : IGameNetwork, IEventListener
    {
        public Action<GameId, BasePacket> OnOutgoingPacket;

        public EventBus<BasePacket> IncomingPackets { get; } = new EventBus<BasePacket>();

        private IGame _game;

        public GameServerNetwork(IGame game)
        {
            _game = game;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void On<T>(Action<T> listener) where T : BasePacket
        {
            IncomingPackets.Register(this, listener);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReceiveInput(GameId sender, BasePacket input)
        {
            input.Sender = _game.Players[sender] ?? new PlayerEntity(_game);
            IncomingPackets.Call(input);
            _game.Entities.DeltaCompression.SendDeltaPackets(input.Sender);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendToServer(BasePacket p, ServerType server)
        {
            IncomingPackets.Call(p); // Hack for local server for now
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendToPlayer(BasePacket p, params PlayerEntity[] players)
        {
            foreach (var player in players)
            {
                if (player == null) continue;
                OnOutgoingPacket?.Invoke(player.EntityId, p);
            }
        }
    }
}