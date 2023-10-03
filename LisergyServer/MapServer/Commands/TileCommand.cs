
using BaseServer.Commands;
using Game;

namespace LisergyServer.Commands
{
    public class TileCommand : Command
    {
        public TileCommand(GameLogic game) : base(game) { }

        public override void Execute(CommandSender sender, CommandArgs args)
        {
            var size = args.Size;
            if (size < 3)
            {
                sender.SendMessage("---- .tile help -----");
                sender.SendMessage(".tile <x> <y> <tileid> - sets the tileid of the given tile");
            }
        }

        public override string GetCommand()
        {
            return "tile";
        }

        public override string Description()
        {
            return "Manipulates tiles";
        }
    }
}
