using Game.Battles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.ClientEvents
{
    [Serializable]
    public class BattleActionEvent : ClientEvent
    {
        public BattleAction Action;
        public string BattleID;

        public override EventID GetID() => EventID.BATTLE_ACTION;
    }
}
