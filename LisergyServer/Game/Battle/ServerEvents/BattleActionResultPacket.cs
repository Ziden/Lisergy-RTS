using Game.Battles;
using Game.BattleTactics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class BattleActionResultPacket : ServerEvent
    {
        public BattleAction [] Actions;
    }
}
