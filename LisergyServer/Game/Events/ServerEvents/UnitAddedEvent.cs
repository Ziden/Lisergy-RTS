using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class UnitVisibleEvent : ServerEvent
    {
        public Unit Unit;

        [NonSerialized]
        public WorldEntity Viewer;
     
        public override EventID GetID() => EventID.UNIT_VISIBLE;
    }
}
