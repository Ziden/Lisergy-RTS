using Game.BattleActions;
using Game.BattleEvents;
using System;

namespace Assets.Code.Battle
{
    public partial class BattlePlayback
    {
        public static event Action<AttackAction> OnAttacked;
        public static event Action<UnitDeadEvent> OnUnitDied;
    }
}
