using Game.Battles;
using Game.Battles.Actions;
using System;

namespace Game.Events
{
    [Serializable]
    public class BattleStartCompleteEvent : ServerEvent
    {
        public string BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;

        public override EventID GetID() => EventID.BATTLE_START_COMPLETE;
    }
}
