using System;

namespace Game.Scheduler
{
    public abstract class FastGameTask : IComparable<FastGameTask>
    {
        private DateTime _start;

        public FastGameTask(TimeSpan delay)
        {
            ID = Guid.NewGuid();
            Delay = delay;
            Start = FastScheduler.Now;
            FastScheduler.Add(this);
        }

        public Guid ID { get; private set; }
        public TimeSpan Delay { get; private set; }
        public DateTime Finish { get; private set; }
        public DateTime Start
        {
            get => _start;
            set {
                _start = value; Finish = _start + Delay;
            }
        }

        public bool Repeat;

        public bool IsDue() => Finish <= FastScheduler.Now;

        public abstract void Execute();

        public int CompareTo(FastGameTask other)
        {
            if (other.ID == this.ID)
                return 0;
            return other.Finish > this.Finish ? -1 : 1;
        }

        public override string ToString()
        {
            return $"<Task {ID.ToString()} Start=<{Start}> End=<{Finish}>>";
        }
    }
}
