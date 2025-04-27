using System;

namespace Game.Scheduler
{
	public abstract class FastGameTask : IComparable<FastGameTask>
	{
		private DateTime _start;

		public bool Repeat;

		public FastGameTask(TimeSpan delay)
		{
			ID = Guid.NewGuid();
			Delay = delay;
			Start = FastScheduler.Now;
			FastScheduler.Add(this);
		}

		public Guid ID { get; }
		public TimeSpan Delay { get; }
		public DateTime Finish { get; private set; }

		public DateTime Start
		{
			get => _start;
			set
			{
				_start = value;
				Finish = _start + Delay;
			}
		}

		public int CompareTo(FastGameTask other)
		{
			if (other.ID == ID)
				return 0;
			return other.Finish > Finish ? -1 : 1;
		}

		public bool IsDue()
		{
			return Finish <= FastScheduler.Now;
		}

		public abstract void Execute();

		public override string ToString()
		{
			return $"<Task {ID.ToString()} Start=<{Start}> End=<{Finish}>>";
		}
	}
}