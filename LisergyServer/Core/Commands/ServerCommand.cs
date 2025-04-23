using System;
using System.Diagnostics;
using BaseServer.Commands;
using Game;
using Game.Engine;

namespace LisergyServer.Commands;

public class ServerCommand : Command
{
	public ServerCommand(LisergyGame game) : base(game)
	{
	}

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
		using (var proc = Process.GetCurrentProcess())
		{
			sender.SendMessage($"Private Allocated (MB): {proc.PrivateMemorySize64 / (1024 * 1024)}");
			sender.SendMessage($"GC Heap Allocated (MB): {GC.GetAllocatedBytesForCurrentThread() / (1024 * 1024)}");
			sender.SendMessage($"GC Total Memory (MB): {GC.GetTotalMemory(false) / (1024 * 1024)}");
		}

		foreach (var m in UnmanagedMemory.GetMetrics()) sender.SendMessage(m);
		//sender.SendMessage($"Server Tick Delay Average: " + SocketServer.Ticker.TPS);
	}
}