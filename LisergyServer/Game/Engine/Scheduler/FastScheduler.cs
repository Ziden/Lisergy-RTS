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

        private static DateTime _schedulerTime;
        private static Dictionary<Guid, FastGameTask> _tasks = new Dictionary<Guid, FastGameTask>();
        private static Dictionary<long, SortedSet<FastGameTask>> _minuteQueues = new Dictionary<long, SortedSet<FastGameTask>>();
        private static SortedSet<FastGameTask> _currentMinuteSet;
        private static long _currentMinute = -1;
        private static FastGameTask _nextTask;

        public static DateTime Now { get => _schedulerTime; }
        internal static TimeSpan NowTimespan { get => Now - Epoch; }
        internal static FastGameTask NextTask { get => _nextTask; }
        public static int PendingTasks { get => _tasks.Values.Count(); }
        public static int AmountQueues { get => _minuteQueues.Values.Count(); }
        public static long CurrentMinute { get => (long)Math.Floor(NowTimespan.TotalMinutes); }

        internal static void Clear()
        {
            _schedulerTime = DateTime.MinValue;
            _tasks = new Dictionary<Guid, FastGameTask>();
            _minuteQueues = new Dictionary<long, SortedSet<FastGameTask>>();
            _currentMinuteSet = null;
            _currentMinute = -1;
            _nextTask = null;
        }

        internal static void SetLogicalTime(DateTime time)
        {
            _schedulerTime = time;
        }

        internal static void Cancel(FastGameTask task)
        {

        }

        internal static void RunTask(FastGameTask task)
        {
            Log.Debug($"Running task {task}");
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
            if (_nextTask == null)
            {
                _nextTask = GetUpdatedCurrentMinuteQueue(CurrentMinute)?.FirstOrDefault();
                if(_nextTask != null)
                    Log.Debug($"[{time}] Set new next task: {_nextTask}");
            }
            RunTasks();
        }

        internal static void RunTasks()
        {
            while (_nextTask != null && _nextTask.IsDue())
            {
                RunTask(_nextTask);
                _nextTask = _currentMinuteSet.FirstOrDefault();
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
            for(var pastMinute = _currentMinute; pastMinute < newCurrentMinute; pastMinute++)
            {
                if(_minuteQueues.TryGetValue(pastMinute, out _currentMinuteSet))
                {
                    while(_currentMinuteSet != null && _currentMinuteSet.Count > 0)
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
        }

        private static SortedSet<FastGameTask> GetUpdatedCurrentMinuteQueue(long newCurrentMinute)
        {
            if (newCurrentMinute != _currentMinute)
            {
                Log.Debug($"Minute {_currentMinute} is now {newCurrentMinute}");
                RunPastTasks(newCurrentMinute);
                _currentMinute = newCurrentMinute;
                if(!_minuteQueues.TryGetValue(_currentMinute, out _currentMinuteSet))
                {
                   //_currentMinuteSet = CreateQueue(_currentMinute);
                }
            }
            return _currentMinuteSet;
        }

        internal static void Add(FastGameTask task)
        {
            _tasks[task.ID] = task;
            var minuteToFinish = (long)Math.Floor((task.Finish - Epoch).TotalMinutes);
            var queue = GetMinuteQueue(minuteToFinish);
            queue.Add(task);
            Log.Debug($"{Now} Registered new task {task} to finish in minute {minuteToFinish} - current {CurrentMinute}");
        }
    }
}
