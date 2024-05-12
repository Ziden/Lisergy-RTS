
using BaseServer.Commands;
using Game;
using Game.Services;

namespace LisergyServer.Commands
{
    public class BattlesCommand : Command
    {
        private BattleService _service;

        public BattlesCommand(LisergyGame game, BattleService battle) : base(game)
        {
            _service = battle;
        }

        public override void Execute(CommandSender sender, CommandArgs args)
        {
            if (args.Size == 0)
            {
                sender.SendMessage("---- .battles help -----");
                sender.SendMessage(".battles list - list all running battles");
            }
            else if (args.GetString(0) == "list")
            {
                foreach (var battle in _service.BattleTasks)
                    sender.SendMessage($"- {battle}");
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
