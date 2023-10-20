using Game.DataTypes;
using Game.Events.Bus;
using Game.Network;
using Game.Systems.Player;
using Game;
using System;
using System.Runtime.CompilerServices;

namespace BaseServer.Core
{
    public class ConnectionHubNetwork : IEventListener
    {
        // TODO: Environment
        public static readonly int WORLD_PORT = 1338;
        public static readonly int HUB_PORT = 1337;

        public Action<ServerType, RoutedPacket> OnOutgoingPacket;

        public EventBus<RoutedPacket> IncomingPackets { get; } = new EventBus<RoutedPacket>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void On<T>(Action<T> listener) where T : BasePacket
        {
            IncomingPackets.Register(this, listener);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReceiveInput(GameId sender, RoutedPacket input)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendToServer(RoutedPacket p, ServerType server)
        {
            OnOutgoingPacket?.Invoke(server, p);
        }
    }
}
