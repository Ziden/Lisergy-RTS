
using BattleServer;
using Game;

namespace LisergyServer.Commands
{
    public class BattlesCommand : Command
    {
        public BattlesCommand(StrategyGame game) : base(game) { }

        public override void Execute(CommandSender sender, CommandArgs args)
        {
            if(args.Size == 0)
            {
                sender.SendMessage("---- .battles help -----");
                sender.SendMessage(".battles list - list all battles");
            } else if(args.GetString(0) == "list")
            {
                var battleListener = this.Game.GetListener<BattleListener>();
                foreach (var battle in battleListener.GetBattles())
                    sender.SendMessage($"- {battle}");
            }
            else if (args.GetString(0) == "clear")
            {
                var battleListener = this.Game.GetListener<BattleListener>();
                battleListener.Wipe();
            }

        }

        public override string GetCommand()
        {
            return "battles";
        }

        public override string Description()
        {
            return "manipulate battles";
        }
    }
}
