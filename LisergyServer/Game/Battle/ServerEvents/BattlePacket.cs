using Game.Battles;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class BattleRefreshPacket: ServerEvent
    {
        public string BattleID;
        public string UserID;

        public BattleRefreshPacket(string battleId, string userId)
        {
            this.BattleID = battleId;
            this.UserID = userId;
        }
    }
}
