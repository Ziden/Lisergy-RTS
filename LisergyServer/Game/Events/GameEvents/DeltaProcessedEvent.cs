﻿using Game.Network;

namespace Game.Events.GameEvents
{
    public class DeltaProcessedEvent : GameEvent
    {
        public IDeltaTrackable Trackable;
    }
}
