using Game;
using Game.Battle;
using Game.Battles;
using Game.Events;
using Game.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleServer
{
    public class BattleListener : EventListener
    {
        public GameWorld World { get; private set; }
        private Dictionary<Guid, TurnBattle> _battles = new Dictionary<Guid, TurnBattle>();

        public TurnBattle GetBattle(string id)
        {
            return _battles[Guid.Parse(id)];
        }

        public void Register(TurnBattle battle)
        {
            _battles.Add(battle.ID, battle);
        }

        public int BattleCount()
        {
            return _battles.Count;
        }

        public BattleListener(GameWorld world)
        {
            World = world;
        }

        public override void Register()
        {
            ServerEventSink.OnBattleStart += OnBattleStart;
            ServerEventSink.OnBattleResult += OnBattleResult;
            Console.WriteLine("[BattleServer] Registered Battle start Listener");
        }

        public override void Unregister()
        {
            ServerEventSink.OnBattleStart -= OnBattleStart;
        }

        public string[] GetInvolveds(BattleResultEvent ev)
        {
            return new string[] { ev.BattleHeader.Attacker.OwnerID, ev.BattleHeader.Defender.OwnerID };
        }

        public void OnBattleStart(BattleStartEvent ev)
        {
            Console.WriteLine($"Received {ev.Attacker} vs {ev.Defender}");
            var battle = new TurnBattle(Guid.Parse(ev.BattleID), ev.Attacker, ev.Defender);
            _battles[battle.ID] = battle;
        }

        public void OnBattleResult(BattleResultEvent ev)
        {
            foreach (var userID in GetInvolveds(ev))
            {
                PlayerEntity pl;
                if (!World.Players.GetPlayer(userID, out pl))
                    continue;

                var battlingParty = pl.Parties.Where(p => p != null && p.BattleID == ev.BattleHeader.BattleID).FirstOrDefault();
                if (battlingParty == null)
                    return;
                battlingParty.BattleID = null;
                // TODO: Copy battle values back to original entities
                pl.Battles.Add(ev);
            }
        }
    }
}
