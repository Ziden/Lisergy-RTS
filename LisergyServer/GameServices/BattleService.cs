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
    internal class BattleFinishedTaskEvent : GameEvent
    {
        public BattleResultPacket ResultPacket;
    }

    /// <summary>
    /// Task to proccess a battle
    /// </summary>
    public class BattleTask : GameTask
    {
        public TurnBattle Battle { get; private set; }
        private readonly IGame _game;

        public BattleTask(IGame game, TurnBattle battle) : base(game, TimeSpan.FromSeconds(3), null)
        {
            Battle = battle;
            _game = game;
        }

        public override void Tick()
        {
            var result = Battle.AutoRun.RunAllRounds();
            _game.Events.Call(new BattleFinishedTaskEvent() { ResultPacket = new BattleResultPacket(Battle.ID, result) });
        }

        public override string ToString() => $"<BattleTask {ID} {Battle}>";
    }

    /// <summary>
    /// Battle server. 
    /// Proccess battles independently in an isolated environment
    /// </summary>
    public class BattleService : IEventListener
    {
        private IGame _game;
        public GameWorld World { get; private set; }
        public Dictionary<GameId, BattleTask> BattleTasks { get; private set; } = new Dictionary<GameId, BattleTask>();
        public Dictionary<GameId, BattleLogPacket> AllBattles { get; private set; } = new Dictionary<GameId, BattleLogPacket>();

        public BattleService(LisergyGame game)
        {
            _game = game;
            World = game.World;
            game.Network.On<BattleTriggeredPacket>(OnBattleTrigger);
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
        public void OnBattleTrigger(BattleTriggeredPacket packet)
        {
            Log.Debug($"Received {packet.Attacker} vs {packet.Defender}");
            var battle = new TurnBattle(packet.BattleID, packet.Attacker, packet.Defender);
            AllBattles[packet.BattleID] = new BattleLogPacket(packet);
            BattleTasks[battle.ID] = new BattleTask(World.Game, battle);
            _game.Network.SendToPlayer(new BattleStartPacket(packet.BattleID, packet.Attacker.Entity, packet.Defender.Entity), GetAllPlayers(battle).ToArray());
        }

        private void OnBattleFinishedProcessing(BattleFinishedTaskEvent ev)
        {
            var fullResultPacket = ev.ResultPacket;
            if (!BattleTasks.TryGetValue(fullResultPacket.Header.BattleID, out var task))
            {
                Log.Error($"Could not find battle {fullResultPacket.Header.BattleID}");
                return;
            }

            AllBattles[fullResultPacket.Header.BattleID].SetTurns(fullResultPacket);
            var summary = new BattleResultSummaryPacket(fullResultPacket.Header);
            var battle = task.Battle;
            foreach (var pl in GetAllPlayers(battle))
            {
                _game.Network.SendToPlayer(summary, GetAllPlayers(battle).ToArray());
                Log.Debug($"Player {pl} completed battle {battle.ID}");
            }
            _game.Network.SendToServer(fullResultPacket, ServerType.WORLD);
            BattleTasks.Remove(fullResultPacket.Header.BattleID);
        }

        public TurnBattle GetRunningBattle(GameId id) => BattleTasks[id].Battle;

        public IEnumerable<PlayerEntity> GetAllPlayers(TurnBattle battle)
        {
            PlayerEntity pl;
            foreach (var userid in new GameId[] { battle.Defender.OwnerID, battle.Attacker.OwnerID })
            {
                if (World.Players.GetPlayer(userid, out pl) && pl.EntityId != GameId.ZERO)
                    yield return pl;
            }
        }
    }
}
