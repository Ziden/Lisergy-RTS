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
        public CompleteBattleHeader BattleHeader;

        public BattleResultSummaryPacket(CompleteBattleHeader header)
        {
            BattleHeader = header;
        }
    }
}
