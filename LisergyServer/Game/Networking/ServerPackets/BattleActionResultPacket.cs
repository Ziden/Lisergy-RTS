using Game.BattleTactics;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class BattleActionResultPacket : ClientPacket
    {
        public ActionResult ActionResult;
    }
}
