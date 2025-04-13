using ClientSDK.SDKEvents;
using Game.Engine.DataTypes;
using Game.Events;
using Game.Systems.Battle.Data;
using Game.Systems.Player;
using System.Collections.Generic;

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

    public class BattleModule(LisergySDK client) : IBattleModule
    {
        public List<BattleHeader> BattleHeaders => client.Server.Player.LocalPlayer.Components.Get<PlayerDataComponent>().BattleHeaders;


        public void Register()
        {
            client.Network.OnInput<BattleHeaderPacket>(OnBattleSummary);
        }

        public void RequestBattleLog(in GameId battleId)
        {

        }

        private void OnBattleSummary(BattleHeaderPacket result)
        {
            if (result.BattleHeader.Attacker.OwnerID == client.Server.Player.PlayerId)
            {
                client.ClientEvents.Call(new OwnBattleFinishedEvent()
                {
                    ImAttacker = true,
                    MyTeam = result.BattleHeader.Attacker,
                    EnemyTeam = result.BattleHeader.Defender,
                    Victory = result.BattleHeader.AttackerWins,
                    BattleId = result.BattleHeader.BattleID
                });
            }
            else if (result.BattleHeader.Defender.OwnerID == client.Server.Player.PlayerId)
            {
                client.ClientEvents.Call(new OwnBattleFinishedEvent()
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
