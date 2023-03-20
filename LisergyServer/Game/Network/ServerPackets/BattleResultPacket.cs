using Game.Battle;
using Game.BattleActions;
using Game.DataTypes;
using Game.Events;
using System;

namespace Game.Network.ServerPackets
{
    /// <summary>
    /// Full battle result.
    /// Only shall be sent the header directly
    /// </summary>
    [Serializable]
    public class BattleResultPacket : ServerPacket
    {
        public BattleHeader BattleHeader;

        public BattleTurnEvent[] Turns;

        public BattleResultPacket(GameId battleID, TurnBattleResult result)
        {
            BattleHeader = new BattleHeader
            {
                BattleID = battleID,
                Date = DateTime.UtcNow,
                Attacker = result.Attacker,
                Defender = result.Defender
            };
            Turns = new BattleTurnEvent[result.Turns.Count];
            BattleHeader.AttackerWins = BattleHeader.Attacker == result.Winner;
            for (int x = 0; x < Turns.Length; x++)
            {
                Turns[x] = new BattleTurnEvent(result.Turns[x]);
            }
        }
    }

    [Serializable]
    public class BattleTurnEvent
    {
        public BattleAction[] Actions;

        public BattleTurnEvent(TurnLog turnLog)
        {
            Actions = new BattleAction[turnLog.Actions.Count];
            for (int x = 0; x < turnLog.Actions.Count; x++)
            {
                Actions[x] = turnLog.Actions[x];
            }
        }
    }
}
