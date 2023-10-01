using System;

namespace Game.Battle.BattleActions
{
    [Serializable]
    public class AttackActionResult : ActionResult
    {
        public ushort Damage;
    }
}
