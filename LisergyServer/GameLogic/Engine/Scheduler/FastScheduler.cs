using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ServerTests")]

namespace Game.Scheduler
{
	// A more complex and optimal version of the scheduler. Needs polishment.
	public static class FastScheduler
	{
		private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private static Dictionary<Guid, FastGameTask> _tasks = new Dictionary<Guid, FastGameTask>();

		private static Dictionary<long, SortedSet<FastGameTask>> _minuteQueues =
			new Dictionary<long, SortedSet<FastGameTask>>();

		private static SortedSet<FastGameTask> _currentMinuteSet;
		private static long _currentMinute = -1;

		public static DateTime Now { get; private set; }

		internal static TimeSpan NowTimespan => Now - Epoch;
		internal static FastGameTask NextTask { get; private set; }

		public static int PendingTasks => _tasks.Values.Count();
		public static int AmountQueues => _minuteQueues.Values.Count();
		public static long CurrentMinute => (long) Math.Floor(NowTimespan.TotalMinutes);

		internal static void Clear()
		{
			Now = DateTime.MinValue;
			_tasks = new Dictionary<Guid, FastGameTask>();
			_minuteQueues = new Dictionary<long, SortedSet<FastGameTask>>();
			_currentMinuteSet = null;
			_currentMinute = -1;
			NextTask = null;
		}

		internal static void SetLogicalTime(DateTime time)
		{
			Now = time;
		}

		internal static void Cancel(FastGameTask task)
		{
		}

		internal static void RunTask(FastGameTask task)
		{
			_currentMinuteSet.Remove(task);
			_tasks.Remove(task.ID);
			task.Execute();
			if (task.Repeat)
			{
				task.Start = Now;
				Add(task);
			}
		}

		public static void Tick(DateTime time)
		{
			SetLogicalTime(time);
			var now = Now;
			if (NextTask == null) NextTask = GetUpdatedCurrentMinuteQueue(CurrentMinute)?.FirstOrDefault();
			RunTasks();
		}

		internal static void RunTasks()
		{
			while (NextTask != null && NextTask.IsDue())
			{
				RunTask(NextTask);
				NextTask = _currentMinuteSet.FirstOrDefault();
			}
		}

		internal static SortedSet<FastGameTask> GetMinuteQueue(long minute)
		{
			SortedSet<FastGameTask> set = null;
			if (!_minuteQueues.TryGetValue(minute, out set))
				set = CreateQueue(minute);
			return set;
		}

		internal static SortedSet<FastGameTask> CreateQueue(long minute)
		{
			if (CurrentMinute > minute)
				throw new Exception($"Trying to read a queue {minute} from the past (current minute: {CurrentMinute})");

			var set = new SortedSet<FastGameTask>();
			_minuteQueues[minute] = set;
			return set;
		}

		private static void RunPastTasks(long newCurrentMinute)
		{
			for (var pastMinute = _currentMinute; pastMinute < newCurrentMinute; pastMinute++)
				if (_minuteQueues.TryGetValue(pastMinute, out _currentMinuteSet))
				{
					while (_currentMinuteSet != null && _currentMinuteSet.Count > 0)
					{
						var next = _currentMinuteSet.First();
						if (next == null)
							break;
						RunTask(next);
					}

					_currentMinuteSet.Clear();
					_currentMinuteSet = null;
				}
		}

		private static SortedSet<FastGameTask> GetUpdatedCurrentMinuteQueue(long newCurrentMinute)
		{
			if (newCurrentMinute != _currentMinute)
			{
				RunPastTasks(newCurrentMinute);
				_currentMinute = newCurrentMinute;
				if (!_minuteQueues.TryGetValue(_currentMinute, out _currentMinuteSet))
				{
					//_currentMinuteSet = CreateQueue(_currentMinute);
				}
			}

			return _currentMinuteSet;
		}

		internal static void Add(FastGameTask task)
		{
			_tasks[task.ID] = task;
			var minuteToFinish = (long) Math.Floor((task.Finish - Epoch).TotalMinutes);
			var queue = GetMinuteQueue(minuteToFinish);
			queue.Add(task);
		}
	}
}