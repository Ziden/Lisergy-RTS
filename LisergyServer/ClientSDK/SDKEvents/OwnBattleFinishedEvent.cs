using ClientSDK.Data;
using Game.Engine.DataTypes;
using Game.Systems.Battle.Data;
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
