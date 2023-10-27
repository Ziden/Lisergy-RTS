namespace Game.Scheduler
{
	public interface ITaskExecutor
	{
		public void Execute(GameTask task);
	}
}