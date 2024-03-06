using System;

namespace Game.Systems.Battle.BattleActions
{
    [Serializable]
    public class AttackActionResult : ActionResult
    {
        public ushort Damage;
    }
}
