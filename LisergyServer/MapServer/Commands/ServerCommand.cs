
using Game;
using Game.Scheduler;
using LisergyServer.Core;

namespace LisergyServer.Commands
{
    public class ServerCommand : Command
    {
        public ServerCommand(StrategyGame game) : base(game) { }

        public override string GetCommand()
        {
            return "sv";
        }

        public override string Description()
        {
            return "Server stuff";
        }

        public override void Execute(CommandSender sender, CommandArgs args)
        {
            sender.SendMessage($"Server Tick Delay Average: "+SocketServer.Ticker.TPS);
        }
    }
}
