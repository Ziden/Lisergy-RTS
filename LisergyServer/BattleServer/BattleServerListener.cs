using Game.Battles;
using Game.Events;
using Game.Listeners;
using System;

namespace BattleServer
{
    public class BattleServerListener : EventListener
    {
        public override void Register()
        {
            NetworkEvents.OnBattleStart += OnBattleStart;
            Console.WriteLine("[BattleServer] Registered Battle start Listener");
        }

        public override void Unregister()
        {
            NetworkEvents.OnBattleStart -= OnBattleStart;
        }

        public void OnBattleStart(BattleStartCompleteEvent ev)
        {
            Console.WriteLine($"Received {ev.Attacker} vs {ev.Defender}");
            ev.Sender.Send(BattleListener.HandleBattle(ev));
        }
    }
}
