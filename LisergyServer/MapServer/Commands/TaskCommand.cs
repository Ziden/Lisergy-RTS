
using Game;
using Game.Scheduler;

namespace LisergyServer.Commands
{
    public class TaskCommand : Command
    {
        public TaskCommand(BlockchainGame game) : base(game) { }

        public override string GetCommand()
        {
            return "task";
        }

        public override string Description()
        {
            return "Show task scheduler info";
        }

        public override void Execute(CommandSender sender, CommandArgs args)
        {
            sender.SendMessage($"Number of queues: {GameScheduler.AmountQueues}");
            sender.SendMessage($"Tasks Pending: {GameScheduler.PendingTasks}");
        }
    }
}
