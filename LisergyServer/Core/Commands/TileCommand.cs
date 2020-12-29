
namespace LisergyServer.Commands
{
    public class TileCommand : Command
    {
        public override void Execute(CommandSender sender, CommandArgs args)
        {
            var size = args.Size();
            if(size < 3)
            {
                sender.SendMessage(".settileid <x> <y> <tileid>");
            }
        }

        public override string GetCommand()
        {
            return "tile";
        }
    }
}
