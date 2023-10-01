using Game.Battle.BattleActions;
using System;

namespace Game.Battle.BattleEvents
{
    [Serializable]
    public class UnitDeadEvent : BattleEvent
    {
        public Guid UnitId;
    }
}
