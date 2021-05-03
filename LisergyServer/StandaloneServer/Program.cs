using Game;
using Game.Events;
using LisergyServer.Core;
using MapServer;

namespace StandaloneServer
{
    class Program
    {
        private class StandalonePlayer : PlayerEntity
        {
            public override bool Online() => true;
            public override void Send<EventType>(EventType ev) => EventEmitter.CallEventFromBytes(this, Serialization.FromEvent(ev));
        }

        private static StandalonePlayer _player = new StandalonePlayer();

        static void Main(string[] args)
        {
            var mapServer = new MapService(1337);
            mapServer.RunServer();
        }
    }
}
