
using BaseServer.Commands;
using BaseServer.Core;
using Game;
using LisergyServer.Core;
using System;
using System.Diagnostics;

namespace LisergyServer.Commands
{
    public class ServerCommand : Command
    {
        public ServerCommand(LisergyGame game) : base(game) { }

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
            using (Process proc = Process.GetCurrentProcess())
            {
                sender.SendMessage($"Private Allocated (MB): {proc.PrivateMemorySize64 / (1024 * 1024)}");
                sender.SendMessage($"GC Heap Allocated (MB): {GC.GetTotalAllocatedBytes() / (1024 * 1024)}");
                sender.SendMessage($"GC Total Memory (MB): {GC.GetTotalMemory(false) / (1024 * 1024)}");
            }
            foreach (var m in UnmanagedMemory.GetMetrics())
            {
                sender.SendMessage(m);
            }
            //sender.SendMessage($"Server Tick Delay Average: " + SocketServer.Ticker.TPS);
        }
    }
}
