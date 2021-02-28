using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Scheduler
{
    public class GameScheduler
    {
        private static DateTime _schedulerTime;
        private Dictionary<Guid, GameTask> _tasks = new Dictionary<Guid, GameTask>();
        private Dictionary<long, SortedSet<GameTask>> _minuteQueues = new Dictionary<long, SortedSet<GameTask>>();
        private SortedSet<GameTask> _currentMinuteSet;
        private long _currentMinute = -1;
        private GameTask _nextTask;

        public static DateTime Now { get => _schedulerTime; }
        public static TimeSpan NowTimespan { get => Now.TimeOfDay; }
        public GameTask NextTask { get => _nextTask; }

        public void SetLogicalTime(DateTime time)
        {
            _schedulerTime = time;
        }

        public void Tick()
        {
            var nowMinute = (long)NowTimespan.TotalMinutes;
            UpdateCurrentMinuteCache(nowMinute);
            if (_currentMinuteSet != null)
            {
                if (_nextTask == null)
                {
                    _nextTask = _currentMinuteSet.FirstOrDefault();
                }
                var wtf1 = _nextTask.Finish;
                var now = GameScheduler.Now;
                var waat = wtf1 <= now;
                while(_nextTask != null && _nextTask.IsDue())
                {
                    _nextTask.Execute();
                    _currentMinuteSet.Remove(_nextTask);
                    _nextTask = _currentMinuteSet.FirstOrDefault();
                }
            }
        }

        private void UpdateCurrentMinuteCache(long currentMinute)
        {
            if (currentMinute != _currentMinute)
            {
                // Running previous minute remaining tasks
                _minuteQueues.TryGetValue(_currentMinute, out _currentMinuteSet);
                if (_currentMinuteSet != null)
                    foreach (var task in _currentMinuteSet)
                        task.Execute();

                _currentMinute = (long)currentMinute;
                _minuteQueues.TryGetValue(_currentMinute, out _currentMinuteSet);
            }
        }

        public SortedSet<GameTask> GetMinuteQueue(long minute)
        {
            SortedSet<GameTask> set = null;
            if (!_minuteQueues.TryGetValue(minute, out set))
            {
                if(NowTimespan.TotalMinutes <= minute) // if its a future queue, we create it
                {
                    set = new SortedSet<GameTask>();
                    _minuteQueues[minute] = set;
                } else
                {
                    throw new Exception("Trying to read a queue from the past");
                }
            }
            return set;
        }

        public void Add(GameTask task)
        {
            _tasks[task.ID] = task;
            var minute = (long)task.Finish.TimeOfDay.TotalMinutes;
            var queue = GetMinuteQueue(minute);
            queue.Add(task);
        }
    }
}
