using Game;
using Game.Battles;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ClientEvents;
using Game.Events.ServerEvents;
using LisergyMessageQueue;
using System.Collections.Generic;

namespace BattleService
{
    public class BattleServerPacketListener : IEventListener
    {
        public Dictionary<string, TurnBattle> RunningBattles = new Dictionary<string, TurnBattle>();

        [EventMethod]
        public void OnBattleRequest(BattleRefreshPacket p)
        {
            Log.Debug($"Request to update battle {p.BattleID}");
            var battle = GetBattle(p.BattleID);
            
            EventMQ.Send(battle.Attacker.OwnerID, new BattleStartPacket()
            {
                Attacker = battle.Attacker,
                Defender = battle.Defender,
                BattleID = p.BattleID
            });

            EventMQ.Send(battle.Defender.OwnerID, new BattleStartPacket()
            {
                Attacker = battle.Attacker,
                Defender = battle.Defender,
                BattleID = p.BattleID
            });
        }

        [EventMethod]
        public void OnBattleStart(BattleStartPacket ev)
        {
            RunningBattles[ev.BattleID] = new TurnBattle(ev.BattleID, ev.Attacker, ev.Defender);
            Log.Info($"Battle {ev.BattleID} started");
        }

        [EventMethod]
        public void OnBattleAction(BattleActionPacket ev)
        {
            var battle = RunningBattles[ev.BattleID];
            var actions = battle.ReceiveAction(ev.Action);
            var actionResultPacket = new BattleActionResultPacket()
            {
                Actions = actions.ToArray()
            };
            EventMQ.Send(ev.BattleID, actionResultPacket);
            Log.Debug($"Action {ev.Action} in battle {ev.BattleID} resulting in {actions.Count} effects");

            if (battle.IsOver)
            {
                var battleResultPacket = new BattleResultPacket(ev.BattleID, battle.Result);
                EventMQ.Send(ev.BattleID, battleResultPacket);
                RunningBattles.Remove(ev.BattleID);
                Log.Info($"Battle {ev.BattleID} finished");
            }
        }

        private TurnBattle GetBattle(string id)
        {
            return RunningBattles[id];
        }
    }
}
