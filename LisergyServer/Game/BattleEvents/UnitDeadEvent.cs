using Game.BattleActions;
using System;

namespace Game.BattleEvents
{
    [Serializable]
    public class UnitDeadEvent : BattleEvent
    {
        public Guid UnitId;
    }
}
