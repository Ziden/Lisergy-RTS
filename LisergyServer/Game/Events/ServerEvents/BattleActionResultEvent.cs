﻿using Game.Battles;
using Game.BattleTactics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class BattleActionResultEvent : ClientEvent
    {
        public ActionResult ActionResult;

        public override EventID GetID() => EventID.BATTLE_ACTION_RESULT;
    }
}