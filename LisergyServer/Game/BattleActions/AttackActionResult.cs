using System;

namespace Game.BattleActions
{
    [Serializable]
    public class AttackActionResult : ActionResult
    {
        public ushort Damage;
    }
}
