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
        private static SortedSet<GameTask> _queue = new SortedSet<GameTask>();
        private static GameTask _nextTask;

        public static DateTime Now { get => _schedulerTime; }
        internal static TimeSpan NowTimespan { get => Now - Epoch; }
        internal static GameTask NextTask { get => _nextTask; }
        public static int PendingTasks { get => _tasks.Values.Count(); }
        public static int AmountQueues { get => _queue.Count; }
        internal static SortedSet<GameTask> Queue { get => _queue; }

        internal static void ForceComplete(GameTask task)
        {
            while (task.Repeat)
            {
                task.Execute();
            }
            DeltaTracker.SendDeltaPackets(task.Creator);  // TODO: Maybe not best place
            _queue.Remove(task);
            _tasks.Remove(task.ID);
        }

        internal static void Clear()
        {
            _schedulerTime = DateTime.MinValue;
            _tasks = new Dictionary<Guid, GameTask>();
            _queue = new SortedSet<GameTask>();
            _nextTask = null;
        }

        internal static void SetLogicalTime(DateTime time)
        {
            _schedulerTime = time;
        }

        internal static void Cancel(GameTask task)
        {
            _tasks.Remove(task.ID);
            _queue.Remove(task);
            task.HasFinished = true;
        }

        internal static void RunTask(GameTask task)
        {
            _queue.Remove(task);
            _tasks.Remove(task.ID);
            task.Execute();
            DeltaTracker.SendDeltaPackets(task.Creator); // TODO: Maybe not best place
            if (task.Repeat)
            {
                task.Start = Now;
                Add(task);
            }
            else
                task.HasFinished = true;
            _nextTask = _queue.FirstOrDefault();
        }

        public static void Tick(DateTime time)
        {
            SetLogicalTime(time);
            var now = Now;
            if (_nextTask == null)
                _nextTask = _queue.FirstOrDefault();
            while (_nextTask != null && _nextTask.IsDue())
                RunTask(_nextTask);
        }

        internal static void Add(GameTask task)
        {
            _tasks[task.ID] = task;
            _queue.Add(task);
            Log.Debug($"{Now} Registered new task {task}");
        }
    }
}
