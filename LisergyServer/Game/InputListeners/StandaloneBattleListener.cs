using Game;
using Game.Battles;
using Game.Events;
using Game.Listeners;
using System;
using System.Linq;

namespace BattleServer
{
    public class StandaloneBattleListener : EventListener
    {
        public GameWorld World { get; private set; }

        public StandaloneBattleListener(GameWorld world)
        {
            World = world;
        }

        public override void Register()
        {
            NetworkEvents.OnBattleStart += OnBattleStart;
            NetworkEvents.OnBattleResult += OnBattleResult;
            Console.WriteLine("[BattleServer] Registered Battle start Listener");
        }

        public override void Unregister()
        {
            NetworkEvents.OnBattleStart -= OnBattleStart;
        }

        public string[] GetInvolveds(BattleResultCompleteEvent ev)
        {
            return new string[] { ev.Attacker.OwnerID, ev.Defender.OwnerID };
        }

        public void OnBattleResult(BattleResultCompleteEvent ev)
        {
            foreach (var userID in GetInvolveds(ev))
            {
                PlayerEntity pl;
                if (!World.Players.GetPlayer(userID, out pl))
                    continue;

                var battlingParty = pl.Parties.Where(p => p != null && p.BattleID == ev.BattleID).FirstOrDefault();
                if (battlingParty == null)
                    return;
                battlingParty.BattleID = null;
                // TODO: Copy battle values back to original entities
                pl.Battles.Add(ev);
            }
        }

        public void OnBattleStart(BattleStartEvent ev)
        {
            Console.WriteLine($"Received {ev.Attacker} vs {ev.Defender}");
            var battle = new TurnBattle(Guid.Parse(ev.BattleID), ev.Attacker, ev.Defender);
            var result = battle.Run();
            var resultEvent = new BattleResultCompleteEvent(result);
            resultEvent.Sender = ev.Sender;
            NetworkEvents.SendBattleResultComplete(resultEvent);
        }

    }
}
