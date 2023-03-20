using Game.Battle;
using Game.DataTypes;
using System;

namespace Game.Events
{
    /// <summary>
    /// Summary of the battle result
    /// </summary>
    [Serializable]
    public class BattleResultSummaryPacket : ServerPacket
    {
        public BattleHeader BattleHeader;

        public BattleResultSummaryPacket(GameId battleID, BattleHeader header)
        {
            BattleHeader = header;
        }
    }
}
