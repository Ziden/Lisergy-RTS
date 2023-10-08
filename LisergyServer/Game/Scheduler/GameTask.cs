using Game.DataTypes;
using Game.Systems.Player;
using System;

namespace Game.Scheduler
{
    public abstract class GameTask : IComparable<GameTask>
    {
        private DateTime _startTime;
        public IGame Game { get; private set; }
        public bool HasFinished { get; internal set; }
        public GameId ID { get; private set; }
        public TimeSpan Delay { get; private set; }
        public DateTime Finish { get; private set; }
        public PlayerEntity Creator { get; private set; }
        public bool Repeat { get; set; }

        public GameTask(IGame game, TimeSpan delay, PlayerEntity creator)
        {
            ID = GameId.Generate();
            Delay = delay;
            Creator = creator;
            Game = game;
            Game.Scheduler.Add(this);
        }

        public abstract void Tick();

        public DateTime Start
        {
            get => _startTime;
            set
            {
                _startTime = value; 
                Finish = _startTime + Delay;
            }
        }

        public bool IsDue() => Finish <= Game.Scheduler.Now;
        public void Cancel() => Game.Scheduler.Cancel(this);
        public int CompareTo(GameTask other) => other.ID == ID ? 0 : other.Finish > Finish ? -1 : 1;
        public override string ToString() => $"<Task {ID} Start={Start} End={Finish}>";
    }
}
