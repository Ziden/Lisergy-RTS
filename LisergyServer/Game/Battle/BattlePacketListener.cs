using Game;
using Game.Battles;
using Game.Entity;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleServer
{
    public class BattlePacketListener : IEventListener
    {
        public GameWorld World { get; private set; }
        private Dictionary<string, TurnBattle> _battlesHappening = new Dictionary<string, TurnBattle>();


        public void Wipe()
        {
            _battlesHappening.Clear();
        }

        public List<TurnBattle> GetBattles()
        {
            return _battlesHappening.Values.ToList();
        }

        [EventMethod]
        public void OnBattleStart(BattleStartEvent ev)
        {
            Console.WriteLine($"Received {ev.Attacker} vs {ev.Defender}");

            ev.Attacker.Entity.BattleID = ev.BattleID;
            ev.Defender.Entity.BattleID = ev.BattleID;

            // register battle
            var battle = new TurnBattle(Guid.Parse(ev.BattleID), ev.Attacker, ev.Defender);
            battle.StartEvent = ev;
            _battlesHappening[battle.ID.ToString()] = battle;
            foreach (var onlinePlayer in GetOnlinePlayers(battle))
                onlinePlayer.Send(ev);

            // run battle
            var result = battle.AutoRun.RunAllRounds();
            var resultEvent = new BattleResultEvent(battle.ID.ToString(), result);

            // For now...
            OnBattleResult(resultEvent);
        }

        [EventMethod]
        public void OnBattleResult(BattleResultEvent ev)
        {
            TurnBattle battle = null;
            if (!_battlesHappening.TryGetValue(ev.BattleHeader.BattleID, out battle))
            {
                ev.Sender.Send(new MessagePopupEvent(PopupType.BAD_INPUT, "Invalid battle"));
                return;
            }
            // handle battle finish
            foreach (var pl in GetAllPlayers(battle))
            {
                if (pl.Online())
                    pl.Send(ev);
                pl.Battles.Add(ev);
                Log.Debug($"Player {pl} completed battle {battle.ID}");
                ev.BattleHeader.Attacker.Entity.BattleID = null;
                ev.BattleHeader.Defender.Entity.BattleID = null;
            }
            _battlesHappening.Remove(ev.BattleHeader.BattleID);
        }
        #region Battle Controller

        public TurnBattle GetBattle(string id)
        {
            return _battlesHappening[id];
        }

        public int BattleCount()
        {
            return _battlesHappening.Count;
        }

        public BattlePacketListener(GameWorld world)
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
                if (World.Players.GetPlayer(userid, out pl) && !Gaia.IsGaia(pl.UserID))
                    yield return pl;
            }
        }

        #endregion
    }
}
