
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
        }
    }
}
