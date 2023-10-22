using Game.Battle.Data;
using Game.DataTypes;
using Game.Network;
using System;

namespace Game.Events
{
    /// <summary>
    /// Summary of the battle result
    /// </summary>
    [Serializable]
    public class BattleResultSummaryPacket : BasePacket, IServerPacket
    {
        public BattleState BattleHeader;

        public BattleResultSummaryPacket(BattleState header)
        {
            BattleHeader = header;
        }
    }
}
