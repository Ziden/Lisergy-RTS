using System;
using Game.Engine.DataTypes;
using Game.Engine.Events;
using Game.Systems.Player;

namespace Game.Engine.Scheduler
{
	public interface IGameTaskParameter
	{
	}

	[Serializable]
	public class GameTaskData
	{
		public TimeSpan Delay;
		public GameId PlayerCreatorId;
		public bool Repeat;
		public DateTime Start;
		public GameId TaskId;
	}

	public class GameTask : IComparable<GameTask>, IDisposable
	{
		public ITaskExecutor Executor;

		[NonSerialized] public IGame Game;

		public GameTask(IGame game, TimeSpan delay, PlayerModel creator, ITaskExecutor executor)
		{
			Pointer = ClassPool<GameTaskData>.Get();
			Pointer.Start = game.GameTime;
			Pointer.Delay = delay;
			Pointer.TaskId = GameId.Generate();
			Pointer.Repeat = false;
			Pointer.PlayerCreatorId = creator?.EntityId ?? GameId.ZERO;
			Executor = executor;
			Game = game;
		}

		public GameTaskData Pointer { get; private set; }
		public DateTime Finish => Pointer.Start + Pointer.Delay;
		public ref DateTime Start => ref Pointer.Start;
		public ref readonly GameId ID => ref Pointer.TaskId;
		public ref bool Repeat => ref Pointer.Repeat;
		public ref TimeSpan Delay => ref Pointer.Delay;
		public GameId Creator => Pointer.PlayerCreatorId;

		public bool HasFinished => Game.GameTime >= Finish;

		public int CompareTo(GameTask other)
		{
			if (other.Pointer == default || Pointer == default) return -1;
			return other.ID == ID ? 0 : other.Finish > Finish ? -1 : 1;
		}

		public void Dispose()
		{
			ClassPool<GameTaskData>.Return(Pointer);
			Pointer = null;
		}

		public virtual void Tick()
		{
			Executor.Execute(this);
		}

		public bool IsDue()
		{
			return Finish <= Game.Scheduler.LogicalTime;
		}

		public void Cancel()
		{
			Game.Scheduler.Cancel(this);
		}

		public override string ToString()
		{
			return Pointer == default ? "<Task Nulled>" : $"<Task {ID} Start={Start} End={Finish} Executor={Executor}>";
		}

		public bool IsDisposed()
		{
			return Pointer == null;
		}
	}
}