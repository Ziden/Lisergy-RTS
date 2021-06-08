using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.GameEvents
{
    public class OffensiveMoveEvent : GameEvent
    {
        public WorldEntity Attacker;
        public WorldEntity Defender;
    }
}
