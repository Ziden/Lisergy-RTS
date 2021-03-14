using Game.Battle;
using Game.Events;
using Game.Listeners;
using System;

namespace BattleServer
{
    public class BattleListener : EventListener
    {
        public override void Register()
        {
            NetworkEvents.OnBattleStart += OnBattleStart;
            Console.WriteLine("Registered Battle Listener");
        }

        public override void Unregister()
        {
            NetworkEvents.OnBattleStart -= OnBattleStart;
        }

        public void OnBattleStart(BattleStartCompleteEvent ev)
        {
            Console.WriteLine($"Received {ev.Attacker} vs {ev.Defender}");
            var battle = new Battle(ev.Attacker, ev.Defender);
            var result = battle.Run();
            var resultEvent = new BattleResultCompleteEvent(result);
            resultEvent.BattleID = ev.BattleID;
            Console.WriteLine($"{result.Winner} won in {result.Turns.Count} turns.");
            ev.Sender.Send(resultEvent);
        }
    }
}
