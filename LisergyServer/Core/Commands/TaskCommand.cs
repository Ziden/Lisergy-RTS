
using BaseServer.Commands;
using Game;
using Game.Scheduler;

namespace LisergyServer.Commands
{
    public class TaskCommand : Command
    {
        public TaskCommand(LisergyGame game) : base(game) { }

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
            var s = Game.Scheduler as GameScheduler;
            sender.SendMessage($"Number of queues: {s.AmountQueues}");
            sender.SendMessage($"Tasks Pending: {s.PendingTasks}");
        }
    }
}
