using System;

namespace Game.Events
{
    [Serializable]
    public abstract class GameEvent: BaseEvent
    {
        private bool _cancelled = false;

        public void Cancel()
        {
            _cancelled = true;
        }
    }
}
