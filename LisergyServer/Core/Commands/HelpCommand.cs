namespace LisergyServer.Commands
{
    public class HelpCommand : Command
    {
        public override string GetCommand()
        {
            return "help";
        }
        public override void Execute(CommandSender sender, CommandArgs args)
        {
            sender.SendMessage("---------- HELP ---------");
            sender.SendMessage(".tasks - show scheduler task counts");
            sender.SendMessage(".tile - tile manipulation");
        }
    }
}
