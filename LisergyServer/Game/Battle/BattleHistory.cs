using Game.DataTypes;
using Game.Network.ServerPackets;
using System.Collections.Generic;

namespace Game.Battle
{
    public class BattleHistory
    {
        private static Dictionary<GameId, BattleLogPacket> Battles = new Dictionary<GameId, BattleLogPacket>();

        public static void Track(BattleStartPacket start)
        {
            Battles[start.BattleID] = new BattleLogPacket(start);
        }

        public static void Track(BattleResultPacket finish)
        {
            Battles[finish.FinalStateHeader.BattleID].SetTurns(finish);
        }

        public static bool TryGetLog(GameId id, out BattleLogPacket log) => Battles.TryGetValue(id, out log);
    }
}
