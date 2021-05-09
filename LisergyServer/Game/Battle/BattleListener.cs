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
            NetworkEvents.OnBattleStart += OnBattleStart;
            NetworkEvents.OnBattleResult += OnBattleResult;
            NetworkEvents.OnBattleAction += OnBattleAction;
            NetworkEvents.OnJoinWorld += OnJoinWorld;
        }

        public override void Unregister()
        {
            NetworkEvents.OnBattleStart -= OnBattleStart;
        }

        public void Wipe()
        {
            _battles.Clear();
        }

        public List<TurnBattle> GetBattles()
        {
            return _battles.Values.ToList();
        }

        public void OnJoinWorld(JoinWorldEvent ev)
        {
            PlayerEntity player = null;
            if (World.Players.GetPlayer(ev.Sender.UserID, out player))
            {
                var battlingParty = player.Parties.FirstOrDefault(p => p != null && p.BattleID != null);
                if (battlingParty != null)
                {
                    var battle = GetBattle(battlingParty.BattleID);
                    player.Send(battle.StartEvent);
                }
            }
        }

        #region Listener
        public void OnBattleAction(BattleActionEvent ev)
        {
            TurnBattle battle = null;
            if (!_battles.TryGetValue(ev.BattleID, out battle))
            {
                ev.Sender.Send(new MessagePopupEvent(PopupType.BAD_INPUT, "Invalid battle"));
                return;
            }
            var actionsHappened = battle.ReceiveAction(ev.Action);
            foreach (var onlinePlayer in GetOnlinePlayers(battle))
                onlinePlayer.Send(new BattleActionResultEvent() { ActionResult = ev.Action.Result });

            TryAutoRun(battle);
        }

        private void TryAutoRun(TurnBattle battle)
        {
            // AutoRun AI
            var autoActions = new List<BattleAction>();
            while (!battle.CurrentActingUnit.Controlled && !battle.IsOver)
                autoActions.AddRange(battle.AutoRun.PlayOneTurn());

            // Send AI results
            if (autoActions.Count > 0)
                foreach (var onlinePlayer in GetOnlinePlayers(battle))
                    foreach (var action in autoActions)
                        onlinePlayer.Send(new BattleActionEvent(battle.ID.ToString(), action));
        }

        public void OnBattleStart(BattleStartEvent ev)
        {
            Console.WriteLine($"Received {ev.Attacker} vs {ev.Defender}");
            var battle = new TurnBattle(Guid.Parse(ev.BattleID), ev.Attacker, ev.Defender);
            battle.StartEvent = ev;
            _battles[battle.ID.ToString()] = battle;
            foreach (var onlinePlayer in GetOnlinePlayers(battle))
                onlinePlayer.Send(ev);
            TryAutoRun(battle);
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
            foreach (var userid in new string[] { battle.Defender.OwnerID, battle.Attacker.OwnerID })
            {
                if (World.Players.GetPlayer(userid, out pl) && pl.Online())
                    yield return pl;
            }
        }

        #endregion
    }
}
