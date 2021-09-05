using Game.Battles;
using Game.Battles.Actions;
using System;

namespace Game.Events
{
    [Serializable]
    public class BattleStartPacket : ServerEvent
    {
        public int X;
        public int Y;
        public string BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;
    }
}
