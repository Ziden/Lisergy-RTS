namespace LisergyServer.Commands
{
    public class HelpCommand : Command
    {
        private CommandExecutor _executor;

        public HelpCommand(CommandExecutor executor) : base(null)
        {
            _executor = executor;
        }

        public override string GetCommand()
        {
            return "help";
        }
        public override void Execute(CommandSender sender, CommandArgs args)
        {
            sender.SendMessage("---------- HELP ---------");
            foreach (var cmd in _executor.GetCommands())
            {
                sender.SendMessage($".{cmd.GetCommand()} - {cmd.Description()}");
            }
        }

        public override string Description()
        {
            return "Show commands";
        }
    }
}
