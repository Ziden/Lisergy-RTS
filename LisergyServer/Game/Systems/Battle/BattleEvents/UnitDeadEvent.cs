using Game.Systems.Battle.BattleActions;
using System;

namespace Game.Systems.Battle.BattleEvents
{
    [Serializable]
    public class UnitDeadEvent : BattleEvent
    {
        public Guid UnitId;
    }
}
