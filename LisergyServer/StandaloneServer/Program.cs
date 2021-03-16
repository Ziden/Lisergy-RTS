using BattleServer;
using Game;
using Game.Events;
using Game.Listeners;
using LisergyServer.Core;
using MapServer;
using System;

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

            // Making a "local" battle server
            NetworkEvents.OnBattleStart += (ev) => EventEmitter.CallEventFromBytes(_player, Serialization.FromEvent(BattleListener.HandleBattle(ev)));

            mapServer.RunServer();
        }
    }
}
