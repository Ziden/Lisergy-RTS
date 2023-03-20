using Game;
using Game.Battle;
using Game.DataTypes;
using Game.ECS;
using Game.Events;
using Game.Events.Bus;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.Network.ServerPackets;
using Game.Player;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleServer
{
    public class BattleService : IEventListener
    {
        public GameWorld World { get; private set; }
        private Dictionary<GameId, TurnBattle> _battlesHappening = new Dictionary<GameId, TurnBattle>();

        public BattleService(StrategyGame game)
        {
            World = game.World;
            game.NetworkEvents.Register<BattleStartPacket>(this, OnBattleStart);
            game.NetworkEvents.Register<BattleResultPacket>(this, OnBattleResult);
        }


        public void Wipe()
        {
            _battlesHappening.Clear();
        }

        public List<TurnBattle> GetBattles()
        {
            return _battlesHappening.Values.ToList();
        }

        [EventMethod]
        public void OnBattleStart(BattleStartPacket ev)
        {
            Console.WriteLine($"Received {ev.Attacker} vs {ev.Defender}");

            var battle = new TurnBattle(ev.BattleID, ev.Attacker, ev.Defender);
            battle.StartEvent = ev;

            ev.Attacker.Entity.BattleGroupLogic.BattleID = ev.BattleID;
            ev.Defender.Entity.BattleGroupLogic.BattleID = ev.BattleID;

            _battlesHappening[battle.ID] = battle;
            foreach (var onlinePlayer in GetOnlinePlayers(battle))
                onlinePlayer.Send(ev);

            battle.Task = new BattleTask(World.Game, battle);
        }

        [EventMethod]
        public void OnBattleResult(BattleResultPacket fullResultPacket)
        {
            TurnBattle battle = null;
            if (!_battlesHappening.TryGetValue(fullResultPacket.BattleHeader.BattleID, out battle))
            {
                fullResultPacket.Sender.Send(new MessagePopupPacket(PopupType.BAD_INPUT, "Invalid battle"));
                return;
            }

            var summary = new BattleResultSummaryPacket(battle.ID, fullResultPacket.BattleHeader);

            foreach (var pl in GetAllPlayers(battle))
            {
                pl.Send(summary);

                // TODO: Send to game logic service
                pl.Battles.Add(fullResultPacket);
                Log.Debug($"Player {pl} completed battle {battle.ID}");
            }

            var atk = fullResultPacket.BattleHeader.Attacker.Entity;
            var def = fullResultPacket.BattleHeader.Defender.Entity;

            var finishEvent = new BattleFinishedEvent(battle, fullResultPacket.BattleHeader, fullResultPacket.Turns);

            if (atk is IEntity e)
            {
                e.Components.CallEvent(finishEvent);
            }
            if (def is IEntity e2)
            {
                e2.Components.CallEvent(finishEvent);
            }

            var atkPacket = atk.GetStatusUpdatePacket();
            var defPacket = def.GetStatusUpdatePacket();

            if (atk.Owner.CanReceivePackets())
            {
                atk.Owner.Send(atkPacket);
                if (!def.BattleGroupLogic.IsDestroyed)
                    atk.Owner.Send(defPacket);
            }

            if (def.Owner.CanReceivePackets())
            {
                if (!atk.BattleGroupLogic.IsDestroyed)
                    def.Owner.Send(atkPacket);
                def.Owner.Send(defPacket);
            }

            _battlesHappening.Remove(fullResultPacket.BattleHeader.BattleID);
        }
        #region Battle Controller

        public TurnBattle GetBattle(GameId id)
        {
            return _battlesHappening[id];
        }

        public int BattleCount()
        {
            return _battlesHappening.Count;
        }
        public IEnumerable<PlayerEntity> GetOnlinePlayers(TurnBattle battle)
        {
            PlayerEntity pl;
            foreach (var userid in new GameId[] { battle.Defender.OwnerID, battle.Attacker.OwnerID })
            {
                if (World.Players.GetPlayer(userid, out pl) && pl.Online())
                    yield return pl;
            }
        }

        public IEnumerable<PlayerEntity> GetAllPlayers(TurnBattle battle)
        {
            PlayerEntity pl;
            foreach (var userid in new GameId[] { battle.Defender.OwnerID, battle.Attacker.OwnerID })
            {
                if (World.Players.GetPlayer(userid, out pl) && !Gaia.IsGaia(pl.UserID))
                    yield return pl;
            }
        }

        #endregion
    }
}
