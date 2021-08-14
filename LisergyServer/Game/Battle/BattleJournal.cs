using Game.Battles;
using Game.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Battle
{
    [Serializable]
    public class BattleHeader
    {
        public bool AttackerWins;
        public string BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;
        public DateTime Date;
    }

    [Serializable]
    public class MiniBattleHeader
    {
        public bool AttackerWins;
        public string BattleID;
        public string attackerId;
        public string defenderId;
        public ushort[] AttackerUnitSpecIds;
        public ushort[] DefenderUnitSpecIds;
        public DateTime Date;
    }



}
