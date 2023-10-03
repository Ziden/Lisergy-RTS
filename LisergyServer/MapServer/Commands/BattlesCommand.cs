
using BaseServer.Commands;
using Game;
using Game.Services;

namespace LisergyServer.Commands
{
    public class BattlesCommand : Command
    {
        private BattleService _service;

        public BattlesCommand(GameLogic game, BattleService battle) : base(game)
        {
            _service = battle;
        }

        public override void Execute(CommandSender sender, CommandArgs args)
        {
            if (args.Size == 0)
            {
                sender.SendMessage("---- .battles help -----");
                sender.SendMessage(".battles list - list all battles");
            }
            else if (args.GetString(0) == "list")
            {
                foreach (var battle in _service.GetBattles())
                    sender.SendMessage($"- {battle}");
            }
            else if (args.GetString(0) == "clear")
            {
                _service.Wipe();
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
