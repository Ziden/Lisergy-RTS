using Game.Engine.DataTypes;
using Game.Engine.Network;
using Game.Systems.Battle;
using Game.Systems.Battle.Data;
using System;

namespace Game.Network.ServerPackets
{
    /// <summary>
    /// Full battle result.
    /// Only shall be sent the header directly, this is to be sent from battle server to map server.
    /// </summary>
    [Serializable]
    public class BattleResultPacket : BasePacket, IServerPacket
    {
        public BattleHeader Header;

        public BattleTurnLog[] Turns;

        public BattleResultPacket(in GameId battleID, TurnBattleRecord result)
        {
            //result.Attacker.AllDead
            Header = new BattleHeader
            {
                BattleID = battleID,
                BattleTime = DateTime.UtcNow,
                Attacker = result.Attacker.TeamData,
                Defender = result.Defender.TeamData
            };
            Turns = new BattleTurnLog[result.Turns.Count];
            Header.AttackerWins = result.Attacker == result.Winner;
            for (int x = 0; x < Turns.Length; x++)
            {
                Turns[x] = new BattleTurnLog(result.Turns[x]);
            }
        }
    }
}
