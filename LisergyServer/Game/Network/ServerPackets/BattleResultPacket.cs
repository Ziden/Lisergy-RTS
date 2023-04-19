using Game.Battle;
using Game.BattleActions;
using Game.DataTypes;
using Game.Events;
using System;

namespace Game.Network.ServerPackets
{
    /// <summary>
    /// Full battle result.
    /// Only shall be sent the header directly, this is to be sent from battle server to map server.
    /// </summary>
    [Serializable]
    public class BattleResultPacket : ServerPacket
    {
        public CompleteBattleHeader FinalStateHeader;

        public BattleTurnLog[] Turns;

        public BattleResultPacket(GameId battleID, TurnBattleRecord result)
        {
            FinalStateHeader = new CompleteBattleHeader
            {
                BattleID = battleID,
                Date = DateTime.UtcNow,
                Attacker = result.Attacker,
                Defender = result.Defender
            };
            Turns = new BattleTurnLog[result.Turns.Count];
            FinalStateHeader.AttackerWins = FinalStateHeader.Attacker == result.Winner;
            for (int x = 0; x < Turns.Length; x++)
            {
                Turns[x] = new BattleTurnLog(result.Turns[x]);
            }
        }
    }
}
