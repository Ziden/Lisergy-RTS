using Game;

namespace BaseServer.Commands
{
    public abstract class Command
    {
        protected StrategyGame Game;

        public abstract string GetCommand();

        public abstract string Description();

        public abstract void Execute(CommandSender sender, CommandArgs args);

        public Command(StrategyGame game)
        {
            Game = game;
        }

        public int GetPermissionLevel()
        {
            return 0;
        }

    }
}
