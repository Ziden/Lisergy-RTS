using Game.Engine.DataTypes;
using Game.Engine.Events;
using Game.Systems.Player;
using System;

namespace Game.Engine.Scheduler
{
    public interface IGameTaskParameter { }

    [Serializable]
    public class GameTaskData
    {
        public DateTime Start;
        public GameId TaskId;
        public GameId PlayerCreatorId;
        public TimeSpan Delay;
        public bool Repeat;
    }

    public unsafe class GameTask : IComparable<GameTask>, IDisposable
    {
        public ITaskExecutor Executor;
        public GameTaskData Pointer { get; private set; }
        public DateTime Finish => Pointer.Start + Pointer.Delay;
        public ref DateTime Start => ref Pointer.Start;
        public ref readonly GameId ID => ref Pointer.TaskId;
        public ref bool Repeat => ref Pointer.Repeat;
        public ref TimeSpan Delay => ref Pointer.Delay;
        public GameId Creator => Pointer.PlayerCreatorId;

        [NonSerialized] public IGame Game;

        public bool HasFinished => Game.GameTime >= Finish;

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

        public virtual void Tick() => Executor.Execute(this);
        public bool IsDue() => Finish <= Game.Scheduler.Now;
        public void Cancel() => Game.Scheduler.Cancel(this);
        public int CompareTo(GameTask other)
        {
            if (other.Pointer == default || Pointer == default) return -1;
            return other.ID == ID ? 0 : other.Finish > Finish ? -1 : 1;
        }

        public override string ToString() => Pointer == default ? "<Task Nulled>" : $"<Task {ID} Start={Start} End={Finish} Executor={Executor}>";

        public void Dispose()
        {
            ClassPool<GameTaskData>.Return(Pointer);
            Pointer = null;
        }
    }
}
