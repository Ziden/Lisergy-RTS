using Game.Events.Bus;
using Game.Events;

namespace Game
{
    public interface IGameNetwork
    {
        public EventBus<BaseEvent> IncomingPackets { get; }
        public EventBus<BaseEvent> OutgoingPackets { get; }
    }

    public class GameNetwork : IGameNetwork
    {
        public EventBus<BaseEvent> IncomingPackets { get; } = new EventBus<BaseEvent>();
        public EventBus<BaseEvent> OutgoingPackets { get; } = new EventBus<BaseEvent>();
    }
}
