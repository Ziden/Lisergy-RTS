using Game.Network.ClientPackets;
using Game;
using Game.Events.ServerEvents;
using System;
using Game.DataTypes;
using ClientSDK.Data;
using Game.Systems.Player;
using ClientSDK.SDKEvents;
using Game.Events;

namespace ClientSDK.Services
{
    /// <summary>
    /// Has battle specific networking implementation
    /// </summary>
    public interface IBattleModule : IClientModule
    {
      
    }

    public class BattleModule : IBattleModule
    {
        private GameClient _client;

        public BattleModule(GameClient client)
        {
            _client = client;
        }

        public void Register()
        {
            _client.Network.On<BattleResultSummaryPacket>(OnBattleSummary);
        }

        private void OnBattleSummary(BattleResultSummaryPacket result)
        {
            if(result.BattleHeader.Attacker.OwnerID == _client.Modules.Player.PlayerId)
            {
                var ev = new OwnBattleFinishedEvent()
                {
                    MyTeam = result.BattleHeader.Attacker,
                    EnemyTeam = result.BattleHeader.Defender,
                    Victory = result.BattleHeader.AttackerWins
                };
            }
        }

       
    }
}
