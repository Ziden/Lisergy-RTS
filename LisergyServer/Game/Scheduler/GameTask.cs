using System;

namespace Game.Scheduler
{
    public abstract class GameTask : IComparable<GameTask>
    {
        public GameTask(TimeSpan delay)
        {
            Start = GameScheduler.Now;
            Delay = delay;
            Finish = Start + delay;
        }

        public Guid ID;
        public DateTime Start { get; private set; }
        public TimeSpan Delay { get; private set; }
        public DateTime Finish { get; private set; }

        public bool IsDue() => Finish <= GameScheduler.Now;

        public abstract void Execute();

        public int CompareTo(GameTask other)
        {
            if (other.ID == this.ID)
                return 0;
            return other.Finish > this.Finish ? -1 : 1;
        }

        public override string ToString()
        {
            return $"<Task {ID.ToString()} End={Finish}>";
        }
    }
}
