using Game.Battles;
using Game.DataTypes;
using System;

namespace Game.Battle
{
    [Serializable]
    public class BattleHeader
    {
        public bool AttackerWins;
        public GameId BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;
        public DateTime Date;
    }

    [Serializable]
    public class MiniBattleHeader
    {
        public bool AttackerWins;
        public GameId BattleID;
        public GameId attackerId;
        public GameId defenderId;
        public ushort[] AttackerUnitSpecIds;
        public ushort[] DefenderUnitSpecIds;
        public DateTime Date;
    }



}
