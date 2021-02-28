using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ServerTests")]
namespace Game.Scheduler
{
    public static class GameScheduler
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static DateTime _schedulerTime;
        private static Dictionary<Guid, GameTask> _tasks = new Dictionary<Guid, GameTask>();
        private static Dictionary<long, SortedSet<GameTask>> _minuteQueues = new Dictionary<long, SortedSet<GameTask>>();
        private static SortedSet<GameTask> _currentMinuteSet;
        private static long _currentMinute = -1;
        private static GameTask _nextTask;

        public static DateTime Now { get => _schedulerTime; }
        internal static TimeSpan NowTimespan { get => Now - Epoch; }
        internal static GameTask NextTask { get => _nextTask; }
        public static int PendingTasks { get => _tasks.Values.Count(); }
        public static int AmountQueues { get => _minuteQueues.Values.Count(); }

        internal static void Clear()
        {
            _schedulerTime = DateTime.MinValue;
            _tasks = new Dictionary<Guid, GameTask>();
            _minuteQueues = new Dictionary<long, SortedSet<GameTask>>();
            _currentMinuteSet = null;
            _currentMinute = -1;
            _nextTask = null;
        }

        internal static void SetLogicalTime(DateTime time)
        {
            _schedulerTime = time;
        }

        internal static void Cancel(GameTask task)
        {

        }

        internal static void RunTask(GameTask task)
        {
            task.Execute();
            _currentMinuteSet.Remove(task);
            _tasks.Remove(task.ID);

        }

        public static void Tick(DateTime time)
        {
            SetLogicalTime(time);
            var now = Now;
            var nowMinute = (long)NowTimespan.TotalMinutes;
            if (_nextTask == null)
                _nextTask = CurrentMinuteQueue(nowMinute)?.FirstOrDefault();
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

        internal static SortedSet<GameTask> GetMinuteQueue(long minute)
        {
            SortedSet<GameTask> set = null;
            if (!_minuteQueues.TryGetValue(minute, out set))
                set = CreateQueue(minute);
            return set;
        }

        internal static SortedSet<GameTask> CreateQueue(long minute)
        {
            if (NowTimespan.TotalMinutes > minute)
                throw new Exception("Trying to read a queue from the past");

            var set = new SortedSet<GameTask>();
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

        private static SortedSet<GameTask> CurrentMinuteQueue(long newCurrentMinute)
        {
            if (newCurrentMinute != _currentMinute)
            {
                RunPastTasks(newCurrentMinute);
                _currentMinute = (long)newCurrentMinute;
                _minuteQueues.TryGetValue(_currentMinute, out _currentMinuteSet);
            }
            return _currentMinuteSet;
        }

        internal static void Add(GameTask task)
        {
            _tasks[task.ID] = task;
            var minute = (long)(task.Finish - Epoch).TotalMinutes;
            var queue = GetMinuteQueue(minute);
            queue.Add(task);
            Log.Debug($"Registered new task {task.ID}");
        }
    }
}
