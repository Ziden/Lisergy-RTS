using Game;
using Game.Battle;
using Game.DataTypes;
using Game.ECS;
using Game.Events;
using Game.Events.Bus;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using Game.Network.ServerPackets;
using Game.Player;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Services
{
    /// <summary>
    /// Battle server. Currently runs same as map server.
    /// Should proccess battles from a queue.
    /// </summary>
    public class BattleService : IEventListener
    {
        public GameWorld World { get; private set; }
        private Dictionary<GameId, TurnBattle> _battlesHappening = new Dictionary<GameId, TurnBattle>();

        public BattleService(StrategyGame game)
        {
            World = game.World;
            game.NetworkEvents.Register<BattleLogRequestPacket>(this, OnBattleRequest);
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
        public void OnBattleRequest(BattleLogRequestPacket p)
        {
            if(BattleHistory.TryGetLog(p.BattleId, out var log))
            {
                p.Sender.Send(log);
            }
        }

        [EventMethod]
        public void OnBattleStart(BattleStartPacket ev)
        {
            Console.WriteLine($"Received {ev.Attacker} vs {ev.Defender}");

            var battle = new TurnBattle(ev.BattleID, ev.Attacker, ev.Defender);
            battle.StartEvent = ev;

            ev.Attacker.Entity.BattleGroupLogic.BattleID = ev.BattleID;
            ev.Defender.Entity.BattleGroupLogic.BattleID = ev.BattleID;

            BattleHistory.Track(ev);

            _battlesHappening[battle.ID] = battle;
            foreach (var p in GetAllPlayers(battle))
            {
                if(p.Online()) 
                    p.Send(ev);
            }

            battle.Task = new BattleTask(World.Game, battle);
        }

        [EventMethod]
        public void OnBattleResult(BattleResultPacket fullResultPacket)
        {
            TurnBattle battle = null;
            if (!_battlesHappening.TryGetValue(fullResultPacket.FinalStateHeader.BattleID, out battle))
            {
                fullResultPacket.Sender.Send(new MessagePopupPacket(PopupType.BAD_INPUT, "Invalid battle"));
                return;
            }

            BattleHistory.Track(fullResultPacket);

            var summary = new BattleResultSummaryPacket(fullResultPacket.FinalStateHeader);

            foreach (var pl in GetAllPlayers(battle))
            {
                pl.Send(summary);

                // TODO: Send to game logic service
                pl.BattleHeaders[fullResultPacket.FinalStateHeader.BattleID] = fullResultPacket.FinalStateHeader;

                Log.Debug($"Player {pl} completed battle {battle.ID}");
            }

            var atk = fullResultPacket.FinalStateHeader.Attacker.Entity;
            var def = fullResultPacket.FinalStateHeader.Defender.Entity;

            var finishEvent = new BattleFinishedEvent(battle, fullResultPacket.FinalStateHeader, fullResultPacket.Turns);

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

            _battlesHappening.Remove(fullResultPacket.FinalStateHeader.BattleID);
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
