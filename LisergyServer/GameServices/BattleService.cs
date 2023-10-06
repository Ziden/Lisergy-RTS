using Game.Battle;
using Game.DataTypes;
using Game.ECS;
using Game.Events;
using Game.Events.Bus;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using Game.Network.ServerPackets;
using Game.Systems.Battler;
using Game.Systems.Player;
using Game.World;
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
        private IGame _game;
        public GameWorld World { get; private set; }
        private Dictionary<GameId, TurnBattle> _battlesHappening = new Dictionary<GameId, TurnBattle>();

        public BattleService(LisergyGame game)
        {
            _game = game;
            World = game.World;
            game.NetworkPackets.Register<BattleLogRequestPacket>(this, OnBattleRequest);
            game.NetworkPackets.Register<BattleStartPacket>(this, OnBattleStart);
            game.NetworkPackets.Register<BattleResultPacket>(this, OnBattleResult);
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
            Log.Debug($"Received {ev.Attacker} vs {ev.Defender}");

            var battle = new TurnBattle(ev.BattleID, ev.Attacker, ev.Defender);
            battle.StartEvent = ev;

            var atkGroup = ev.Attacker.Entity.Components.Get<BattleGroupComponent>();
            var defGroup = ev.Defender.Entity.Components.Get<BattleGroupComponent>();

            atkGroup.BattleID = ev.BattleID;
            defGroup.BattleID = ev.BattleID;

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
                pl.Data.BattleHeaders[fullResultPacket.FinalStateHeader.BattleID] = fullResultPacket.FinalStateHeader;
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

            var atkGroup = atk.Components.Get<BattleGroupComponent>();
            var defGroup = def.Components.Get<BattleGroupComponent>();

            var atkPlayer = _game.Players.GetPlayer(atk.OwnerID);
            var defPlayer = _game.Players.GetPlayer(def.OwnerID);

            // remove all this updates
            if (atkPlayer != null && atkPlayer.CanReceivePackets())
            {
                atkPlayer.Send(atk.GetUpdatePacket(atkPlayer));
                if (!_game.Logic.BattleGroup(def).IsDestroyed)
                    atkPlayer.Send(def.GetUpdatePacket(atkPlayer));
            }

            if (defPlayer != null && defPlayer.CanReceivePackets())
            {
                if (!_game.Logic.BattleGroup(def).IsDestroyed)
                    defPlayer.Send(atk.GetUpdatePacket(defPlayer));
                defPlayer.Send(def.GetUpdatePacket(defPlayer));
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
                if (World.Players.GetPlayer(userid, out pl) && pl.EntityId != GameId.ZERO)
                    yield return pl;
            }
        }

        #endregion
    }
}
