using Game;
using Game.Battles;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleServer
{
    public class BattleListener : IEventListener
    {
        public GameWorld World { get; private set; }
        private Dictionary<string, TurnBattle> _battles = new Dictionary<string, TurnBattle>();


        public void Wipe()
        {
            _battles.Clear();
        }

        public List<TurnBattle> GetBattles()
        {
            return _battles.Values.ToList();
        }

        [EventMethod]
        public void OnBattleStart(BattleStartEvent ev)
        {
            Console.WriteLine($"Received {ev.Attacker} vs {ev.Defender}");

            // register battle
            var battle = new TurnBattle(Guid.Parse(ev.BattleID), ev.Attacker, ev.Defender);
            battle.StartEvent = ev;
            _battles[battle.ID.ToString()] = battle;
            foreach (var onlinePlayer in GetOnlinePlayers(battle))
                onlinePlayer.Send(ev);

            // run battle
            var result = battle.AutoRun.RunAllRounds();
            var resultEvent = new BattleResultEvent(battle.ID.ToString(), result);

            // handle battle finish
            foreach (var pl in GetAllPlayers(battle))
            {
                if (pl.Online())
                    pl.Send(resultEvent);
                pl.Battles.Add(resultEvent);
                var battlingParty = pl.Parties.Where(p => p != null && p.BattleID == resultEvent.BattleHeader.BattleID).FirstOrDefault();
                if (battlingParty != null)
                    battlingParty.BattleID = null;
                else
                    Log.Error($"Party {battlingParty} was not part of battle {battle}");
            }
        }

        [EventMethod]
        public void OnBattleResult(BattleResultEvent ev)
        {
            TurnBattle battle = null;
            if (!_battles.TryGetValue(ev.BattleHeader.BattleID, out battle))
            {
                ev.Sender.Send(new MessagePopupEvent(PopupType.BAD_INPUT, "Invalid battle"));
                return;
            }
            foreach (var pl in GetOnlinePlayers(battle))
            {
                var battlingParty = pl.Parties.Where(p => p != null && p.BattleID == ev.BattleHeader.BattleID).FirstOrDefault();
                if (battlingParty == null)
                    throw new Exception($"Player {pl} in {this} without a party assigned");
                pl.Battles.Add(ev);
            }
            _battles.Remove(ev.BattleHeader.BattleID);
        }
        #region Battle Controller

        public TurnBattle GetBattle(string id)
        {
            return _battles[id];
        }

        public int BattleCount()
        {
            return _battles.Count;
        }

        public BattleListener(GameWorld world)
        {
            World = world;
        }

        public IEnumerable<PlayerEntity> GetOnlinePlayers(TurnBattle battle)
        {
            PlayerEntity pl;
            foreach (var userid in new string[] { battle.Defender.OwnerID, battle.Attacker.OwnerID })
            {
                if (World.Players.GetPlayer(userid, out pl) && pl.Online())
                    yield return pl;
            }
        }

        public IEnumerable<PlayerEntity> GetAllPlayers(TurnBattle battle)
        {
            PlayerEntity pl;
            foreach (var userid in new string[] { battle.Defender.OwnerID, battle.Attacker.OwnerID })
            {
                if (World.Players.GetPlayer(userid, out pl))
                    yield return pl;
            }
        }

        #endregion
    }
}
