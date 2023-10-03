using Game.Network;
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
        private static Dictionary<Guid, GameTask> _tasks = new Dictionary<Guid, GameTask>();

        public static DateTime Now { get; private set; }
        internal static TimeSpan NowTimespan => Now - Epoch;
        internal static GameTask NextTask { get; private set; }
        public static int PendingTasks => _tasks.Values.Count();
        public static int AmountQueues => Queue.Count;
        internal static SortedSet<GameTask> Queue { get; private set; } = new SortedSet<GameTask>();

        internal static void ForceComplete(GameTask task)
        {
            while (task.Repeat)
            {
                task.Tick();
            }
            DeltaTracker.SendDeltaPackets(task.Creator);  // TODO: Maybe not best place
            _ = Queue.Remove(task);
            _ = _tasks.Remove(task.ID);
        }

        internal static void Clear()
        {
            Now = DateTime.MinValue;
            _tasks = new Dictionary<Guid, GameTask>();
            Queue = new SortedSet<GameTask>();
            NextTask = null;
        }

        internal static void SetLogicalTime(DateTime time)
        {
            Now = time;
        }

        internal static void Cancel(GameTask task)
        {
            _ = _tasks.Remove(task.ID);
            _ = Queue.Remove(task);
            task.HasFinished = true;
        }

        internal static void RunTask(GameTask task)
        {
            _ = Queue.Remove(task);
            _ = _tasks.Remove(task.ID);
            task.Tick();
            DeltaTracker.SendDeltaPackets(task.Creator); // TODO: Maybe not best place
            if (task.Repeat)
            {
                task.Start = Now;
                Add(task);
            }
            else
            {
                task.HasFinished = true;
            }

            NextTask = Queue.FirstOrDefault();
        }

        public static void Tick(DateTime time)
        {
            SetLogicalTime(time);
            _ = Now;
            if (NextTask == null)
            {
                NextTask = Queue.FirstOrDefault();
            }

            while (NextTask != null && NextTask.IsDue())
            {
                RunTask(NextTask);
            }
        }

        internal static void Add(GameTask task)
        {
            _tasks[task.ID] = task;
            _ = Queue.Add(task);
            Log.Debug($"{Now} Registered new task {task}");
        }
    }
}
