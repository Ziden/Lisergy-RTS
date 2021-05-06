/*
using Game.Battles;
using Game.Events;
using Game.Listeners;
using System;
using System.Collections.Generic;

namespace BattleServer
{
    public class StandaloneBattleListener : EventListener
    {
        private List<TurnBattle> _pendingList = new List<TurnBattle>();
        private Dictionary<Guid, TurnBattle> _pending = new Dictionary<Guid, TurnBattle>();

        public override void Register()
        {
            NetworkEvents.OnBattleStart += OnBattleStart;
            Console.WriteLine("[BattleServer] Registered Battle start Listener");
        }

        public override void Unregister()
        {
            NetworkEvents.OnBattleStart -= OnBattleStart;
        }

        public void ProccessNextBattle()
        {
            var next = _pendingList[0];
            _pendingList.RemoveAt(0);
            _pending.Remove(next.ID);

            var result = next.Run();
            var resultEvent = new BattleResultCompleteEvent(result);
            NetworkEvents.SendBattleResultComplete(resultEvent);
        }

        public void OnBattleStart(BattleStartEvent ev)
        {
            Console.WriteLine($"Received {ev.Attacker} vs {ev.Defender}");
            var battle = new TurnBattle(Guid.Parse(ev.BattleID), ev.Attacker, ev.Defender);
            _pending.Add(battle.ID, battle);
            _pendingList.Add(battle);
        }
    }
}
*/