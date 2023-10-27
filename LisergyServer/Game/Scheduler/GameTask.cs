using Game.DataTypes;
using Game.Systems.Player;
using System;
using System.Runtime.InteropServices;

namespace Game.Scheduler
{
    public interface IGameTaskParameter {}
    
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct GameTaskData
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
        
        private GameTaskData* _data;
        public IntPtr Pointer => (IntPtr)_data;
        
        public DateTime Finish => _data->Start + _data->Delay;
        public ref DateTime Start => ref _data->Start;
        public ref readonly GameId ID => ref _data->TaskId;
        public ref bool Repeat => ref _data->Repeat;
        public ref TimeSpan Delay => ref _data->Delay;
        public PlayerEntity Creator => Game.Players[_data->PlayerCreatorId];
        
        [NonSerialized] public IGame Game;
        
        public bool HasFinished => Game.GameTime >= Finish;

        public GameTask(IGame game, TimeSpan delay, PlayerEntity creator, ITaskExecutor executor)
        {
            _data = UnmanagedMemory.Alloc<GameTaskData>();
            _data->Start = game.GameTime;
            _data->Delay = delay;
            _data->TaskId = GameId.Generate();
            _data->Repeat = false;
            _data->PlayerCreatorId = creator?.EntityId ?? GameId.ZERO;
            Executor = executor;
            Game = game;
        }

        public virtual void Tick() => Executor.Execute(this);
        public bool IsDue() => Finish <= Game.Scheduler.Now;
        public void Cancel() => Game.Scheduler.Cancel(this);
        public int CompareTo(GameTask other) => other.ID == ID ? 0 : other.Finish > Finish ? -1 : 1;
        public override string ToString() => $"<Task {ID} Start={Start} End={Finish} Executor={Executor}>";

        public void Dispose() => UnmanagedMemory.FreeForReuse(Pointer);
    }
}
