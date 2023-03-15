using Game;
using Game.Battle;
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

            // register battle
            var battle = new TurnBattle(ev.BattleID, ev.Attacker, ev.Defender);
            battle.StartEvent = ev;
            ev.Attacker.Entity.OnBattleStarted(battle);
            ev.Defender.Entity.OnBattleStarted(battle);

            _battlesHappening[battle.ID] = battle;
            foreach (var onlinePlayer in GetOnlinePlayers(battle))
                onlinePlayer.Send(ev);

            battle.Task = new BattleTask(World.Game, battle);
        }

        [EventMethod]
        public void OnBattleResult(BattleResultPacket ev)
        {
            TurnBattle battle = null;
            if (!_battlesHappening.TryGetValue(ev.BattleHeader.BattleID, out battle))
            {
                ev.Sender.Send(new MessagePopupPacket(PopupType.BAD_INPUT, "Invalid battle"));
                return;
            }
            
  
            foreach (var pl in GetAllPlayers(battle))
            {
                pl.Send(ev);
                pl.Battles.Add(ev);
                Log.Debug($"Player {pl} completed battle {battle.ID}");
            }

            var atk = ev.BattleHeader.Attacker.Entity as IBattleable;
            var def = ev.BattleHeader.Defender.Entity as IBattleable;

            atk.OnBattleFinished(battle, ev.BattleHeader, ev.Turns);
            def.OnBattleFinished(battle, ev.BattleHeader, ev.Turns);

            var atkPacket = atk.GetUpdatePacket();
            var defPacket = def.GetUpdatePacket();

            if (atk.Owner.CanReceivePackets())
            {
                atk.Owner.Send(atkPacket);
                if(!def.IsDestroyed)
                    atk.Owner.Send(defPacket);
            }

            if(def.Owner.CanReceivePackets())
            {
                if(!atk.IsDestroyed)
                    def.Owner.Send(atkPacket);
                def.Owner.Send(defPacket);
            }

            _battlesHappening.Remove(ev.BattleHeader.BattleID);
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
