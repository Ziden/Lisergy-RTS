using System;

namespace Game.Events
{
    /// <summary>
    /// Game Logic Events
    /// </summary>
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
