using ClientSDK.Data;
using Game.Battle;
using Game.Battle.Data;
using Game.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSDK.SDKEvents
{
    /// <summary>
    /// Called whenever a battle owned by the player is finished.
    /// </summary>
    public class OwnBattleFinishedEvent : IClientEvent
    {
        public bool ImAttacker;
        public BattleTeamData MyTeam;
        public BattleTeamData EnemyTeam;
        public bool Victory;
        public GameId BattleId;
    }
}
