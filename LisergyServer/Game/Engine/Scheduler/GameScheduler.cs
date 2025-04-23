using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Game.Engine.DataTypes;

[assembly: InternalsVisibleTo("ServerTests")]

namespace Game.Engine.Scheduler
{
    /// <summary>
    ///     Responsible for controlling the game tasks.
    /// </summary>
    public interface IGameScheduler
	{
		public DateTime LogicalTime { get; }
		public GameTask GetTask(GameId id);
		public void Add(GameTask task);
		void Cancel(GameTask task);
	}

	public class GameScheduler : IGameScheduler
	{
		private readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		private Dictionary<GameId, GameTask> _tasks = new Dictionary<GameId, GameTask>();
		internal TimeSpan NowTimespan => LogicalTime - Epoch;
		internal GameTask NextTask { get; private set; }
		internal SortedSet<GameTask> Queue { get; private set; } = new SortedSet<GameTask>();

		public int PendingTasks => _tasks.Values.Count();
		public int AmountQueues => Queue.Count;

		public DateTime LogicalTime { get; private set; }


		public GameTask GetTask(GameId id)
		{
			_tasks.TryGetValue(id, out var task);
			return task;
		}

		public void Cancel(GameTask task)
		{
			_ = _tasks.Remove(task.ID);
			_ = Queue.Remove(task);
			task.Delay = TimeSpan.FromSeconds(0);
			task.Dispose();
			if (NextTask == task) NextTask = Queue.FirstOrDefault();
		}

		public void Add(GameTask task)
		{
			task.Start = LogicalTime;
			_tasks[task.ID] = task;
			_ = Queue.Add(task);
		}

		internal void ForceComplete(GameTask task)
		{
			while (task.Repeat) task.Tick();
			task.Game.Network.DeltaCompression.SendAllModifiedEntities(task.Creator); // TODO: Maybe not best place
			Cancel(task);
		}

		internal void Clear()
		{
			LogicalTime = DateTime.MinValue;
			_tasks = new Dictionary<GameId, GameTask>();
			Queue = new SortedSet<GameTask>();
			NextTask = null;
		}

		internal void SetLogicalTime(DateTime time)
		{
			LogicalTime = time;
		}

		internal void RunTask(GameTask task)
		{
			task.Tick();
			_ = Queue.Remove(task);
			_ = _tasks.Remove(task.ID);
			task.Game.Network.DeltaCompression.SendAllModifiedEntities(task.Creator); // TODO: Maybe not best place
			if (task.Repeat)
			{
				task.Start = LogicalTime;
				Add(task);
			}
			else
			{
				task.Dispose();
			}

			NextTask = Queue.FirstOrDefault();
		}

		public void Tick(DateTime time)
		{
			SetLogicalTime(time);
			
			if (NextTask == null) NextTask = Queue.FirstOrDefault();

			while (NextTask != null && NextTask.IsDue()) RunTask(NextTask);
		}
	}
}