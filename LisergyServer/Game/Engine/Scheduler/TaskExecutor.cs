namespace Game.Engine.Scheduler
{
    public interface ITaskExecutor
    {
        public void Execute(GameTask task);
    }
}