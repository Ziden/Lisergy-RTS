﻿using Game.Network.ClientPackets;
using Game;
using Game.Events.ServerEvents;
using System;
using Game.DataTypes;
using ClientSDK.Data;
using Game.Systems.Player;
using ClientSDK.SDKEvents;
using Game.Events;
using System.Collections.Generic;
using Game.Battle.Data;

namespace ClientSDK.Services
{
    /// <summary>
    /// Has battle specific networking implementation
    /// </summary>
    public interface IBattleModule : IClientModule
    {
        /// <summary>
        /// Requests a full battle log to view the full battle
        /// Will fire a BattleLogReceivedEvent
        /// </summary>
        void RequestBattleLog(in GameId battleId);

        /// <summary>
        /// Gets the list of battle headers for this player
        /// </summary>
        List<BattleHeader> BattleHeaders { get; }
    }

    public class BattleModule : IBattleModule
    {
        private GameClient _client;

        public BattleModule(GameClient client)
        {
            _client = client;
        }

        public List<BattleHeader> BattleHeaders => _client.Modules.Player.LocalPlayer.Data.BattleHeaders;


        public void Register()
        {
            _client.Network.On<BattleHeaderPacket>(OnBattleSummary);
        }

        public void RequestBattleLog(in GameId battleId)
        {

        }

        private void OnBattleSummary(BattleHeaderPacket result)
        {
            if(result.BattleHeader.Attacker.OwnerID == _client.Modules.Player.PlayerId)
            {
                _client.ClientEvents.Call(new OwnBattleFinishedEvent()
                {
                    ImAttacker = true,
                    MyTeam = result.BattleHeader.Attacker,
                    EnemyTeam = result.BattleHeader.Defender,
                    Victory = result.BattleHeader.AttackerWins,
                    BattleId = result.BattleHeader.BattleID
                });
            } else if (result.BattleHeader.Defender.OwnerID == _client.Modules.Player.PlayerId)
            {
                _client.ClientEvents.Call(new OwnBattleFinishedEvent()
                {
                    ImAttacker = false,
                    MyTeam = result.BattleHeader.Defender,
                    EnemyTeam = result.BattleHeader.Attacker,
                    Victory = !result.BattleHeader.AttackerWins,
                    BattleId = result.BattleHeader.BattleID
                });
            }
        }
    }
}