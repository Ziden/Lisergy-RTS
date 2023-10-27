using Game.Battle;
using Game.DataTypes;
using Game.Events;
using Game.Events.Bus;
using Game.Network.ClientPackets;
using Game.Network.ServerPackets;
using Game.Scheduler;
using Game.Systems.Player;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Services
{
    /// <summary>
    /// Internal Service event to tell the battle service a battle finished processing
    /// </summary>
    internal class BattleFinishedTaskEvent : IGameEvent
    {
        public BattleResultPacket ResultPacket;
        public TurnBattle Battle;
    }

    /// <summary>
    /// Task to proccess a battle
    /// </summary>
    public class BattleTaskExecutor : ITaskExecutor
    {
        public TurnBattle Battle;

        public BattleTaskExecutor(TurnBattle battle) {  Battle = battle; }
        
        public void Execute(GameTask task)
        {
            task.Game.Events.Call(new BattleFinishedTaskEvent()
            {
                Battle = Battle,
                ResultPacket = new BattleResultPacket(Battle.ID, Battle.AutoRun.RunAllRounds())
            });
        }
    }

    /// <summary>
    /// Battle server. 
    /// Proccess battles independently in an isolated environment
    /// </summary>
    public class BattleService : IEventListener
    {
        private IGame _game;
        public Dictionary<GameId, GameTask> BattleTasks { get; private set; } = new Dictionary<GameId, GameTask>();
        public Dictionary<GameId, BattleLogPacket> AllBattles { get; private set; } = new Dictionary<GameId, BattleLogPacket>();

        public BattleService(LisergyGame game)
        {
            _game = game;
            game.Network.On<BattleQueuedPacket>(OnBattleTrigger);
            game.Network.On<BattleLogRequestPacket>(OnBattleRequest);
            game.Events.Register<BattleFinishedTaskEvent>(this, OnBattleFinishedProcessing);
        }

        /// <summary>
        /// Called whenever a player requests for a full battle log
        /// </summary>
        public void OnBattleRequest(BattleLogRequestPacket p)
        {
            if (AllBattles.TryGetValue(p.BattleId, out var log)) _game.Network.SendToPlayer(log, p.Sender); 
        }

        /// <summary>
        /// Triggered when the service receives a new battle to be proccesed
        /// </summary>
        public void OnBattleTrigger(BattleQueuedPacket packet)
        {
            _game.Log.Debug($"Received {packet.Attacker} vs {packet.Defender}");
            var battle = new TurnBattle(packet.BattleID, packet.Attacker, packet.Defender);
            AllBattles[packet.BattleID] = new BattleLogPacket(packet);

            var attackerPlayer = _game.Players[packet.Attacker.OwnerID];
            var executor = new BattleTaskExecutor(battle);
            var task = new GameTask(_game, TimeSpan.FromSeconds(3), attackerPlayer, executor);
            _game.Scheduler.Add(task);
            BattleTasks[battle.ID] = task;
            _game.Network.SendToPlayer(new BattleStartPacket(packet.BattleID, packet.Position, packet.Attacker, packet.Defender), GetAllPlayers(battle).ToArray());
        }

        /// <summary>
        /// A battle finished processing from the queue. 
        /// Needs to 
        /// </summary>
        /// <param name="ev"></param>
        private void OnBattleFinishedProcessing(BattleFinishedTaskEvent ev)
        {
            var fullResultPacket = ev.ResultPacket;
            if (!BattleTasks.TryGetValue(fullResultPacket.Header.BattleID, out var task))
            {
                _game.Log.Error($"Could not find battle {fullResultPacket.Header.BattleID}");
                return;
            }

            AllBattles[fullResultPacket.Header.BattleID].SetTurns(fullResultPacket);
            var header = new BattleHeaderPacket(fullResultPacket.Header);
            var battle = ev.Battle;
            foreach (var pl in GetAllPlayers(battle))
            {
                _game.Network.SendToPlayer(header, GetAllPlayers(battle).ToArray());
                _game.Log.Debug($"Player {pl} completed battle {battle.ID}");
            }
            _game.Network.SendToServer(fullResultPacket, ServerType.WORLD);
            BattleTasks.Remove(fullResultPacket.Header.BattleID);
        }

        public TurnBattle GetRunningBattle(GameId id) => ((BattleTaskExecutor)BattleTasks[id].Executor).Battle;

        public IEnumerable<PlayerEntity> GetAllPlayers(TurnBattle battle)
        {
            PlayerEntity pl;
            foreach (var userid in new GameId[] { battle.Defender.OwnerID, battle.Attacker.OwnerID })
            {
                if (_game.Players.GetPlayer(userid, out pl) && pl.EntityId != GameId.ZERO)
                    yield return pl;
            }
        }
    }
}
