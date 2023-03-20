using Game.Player;
using System;

namespace Game.Scheduler
{
    public abstract class GameTask : IComparable<GameTask>
    {
        private DateTime _start;
        //private readonly DateTime _executionTime;

        public GameTask(TimeSpan delay, PlayerEntity creator)
        {
            ID = Guid.NewGuid();
            Delay = delay;
            Creator = creator;
            Start = GameScheduler.Now;
            GameScheduler.Add(this);
        }

        internal bool HasFinished;
        public Guid ID { get; private set; }
        public TimeSpan Delay { get; private set; }
        public DateTime Finish { get; private set; }

        public PlayerEntity Creator { get; private set; }

        public DateTime Start
        {
            get => _start;
            set
            {
                _start = value; Finish = _start + Delay;
            }
        }


        public bool Repeat;

        public bool IsDue()
        {
            return Finish <= GameScheduler.Now;
        }

        public abstract void Execute();

        public void Cancel()
        {
            GameScheduler.Cancel(this);
        }

        public int CompareTo(GameTask other)
        {
            return other.ID == ID ? 0 : other.Finish > Finish ? -1 : 1;
        }

        public override string ToString()
        {
            return $"<Task {ID} Start=<{Start}> End=<{Finish}>>";
        }
    }
}
