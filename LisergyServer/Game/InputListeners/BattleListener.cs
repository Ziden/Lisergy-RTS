using Game;
using Game.Battles;
using Game.Events;
using Game.Events.ClientEvents;
using Game.Events.ServerEvents;
using Game.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleServer
{
    public class BattleListener : EventListener
    {
        public GameWorld World { get; private set; }
        private Dictionary<string, TurnBattle> _battles = new Dictionary<string, TurnBattle>();

        public override void Register()
        {
            ServerEventSink.OnBattleStart += OnBattleStart;
            ServerEventSink.OnBattleResult += OnBattleResult;
            ServerEventSink.OnBattleAction += OnBattleAction;
        }

        public override void Unregister()
        {
            ServerEventSink.OnBattleStart -= OnBattleStart;
        }

        #region Listener
        public void OnBattleAction(BattleActionEvent ev)
        {
            TurnBattle battle = null;
            if(!_battles.TryGetValue(ev.BattleID, out battle))
            {
                ev.Sender.Send(new MessagePopupEvent(PopupType.BAD_INPUT, "Invalid battle"));
                return;
            }
            battle.ReceiveAction(ev.Action);
            foreach(var onlinePlayer in GetOnlinePlayers(battle))
                onlinePlayer.Send(new BattleActionResultEvent() { ActionResult = ev.Action.Result });
        }

        public void OnBattleStart(BattleStartEvent ev)
        {
            Console.WriteLine($"Received {ev.Attacker} vs {ev.Defender}");
            var battle = new TurnBattle(Guid.Parse(ev.BattleID), ev.Attacker, ev.Defender);
            _battles[battle.ID.ToString()] = battle;
        }

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

        #endregion

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
            foreach (var userid in new string []{ battle.Defender.OwnerID , battle.Attacker.OwnerID })
            {
                if(World.Players.GetPlayer(userid, out pl) && pl.Online())
                    yield return pl; 
            }
        }

        #endregion
    }
}
