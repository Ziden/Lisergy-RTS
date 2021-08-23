using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.GameEvents
{
    public class PlayerJoinedEvent : GameEvent
    {
        public PlayerEntity Player;

        public PlayerJoinedEvent(PlayerEntity p)
        {
            this.Player = p;
        }
    }
}
