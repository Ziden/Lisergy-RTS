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
        public BattleHeaderData BattleHeader;

        public BattleResultSummaryPacket(BattleHeaderData header)
        {
            BattleHeader = header;
        }
    }
}
