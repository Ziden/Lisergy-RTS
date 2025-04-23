using System;

namespace Game.Engine
{
	public interface IGameLog
	{
		public ConsoleColor DebugColor { get; set; }
		public ConsoleColor InfoColor { get; set; }
		public ConsoleColor ErrorColor { get; set; }

		public void Debug(string msg);
		public void Info(string msg);
		public void Error(string msg);
	}

	public class GameLog : IGameLog
	{
		public Action<string> _Debug = Console.WriteLine;
		public Action<string> _Error = Console.Error.WriteLine;

		public Action<string> _Info = msg => { Console.WriteLine(msg); };

		public GameLog(string tag)
		{
			Tag = tag + " ";
		}

		public string Tag { get; }
		public ConsoleColor DebugColor { get; set; } = ConsoleColor.Gray;
		public ConsoleColor InfoColor { get; set; } = ConsoleColor.White;
		public ConsoleColor ErrorColor { get; set; } = ConsoleColor.Red;

		public void Debug(string msg)
		{
#if DEBUG
			Console.ForegroundColor = DebugColor;
			_Debug(Tag + msg);
#endif
		}

		public void Info(string msg)
		{
			Console.ForegroundColor = InfoColor;
			_Info(Tag + msg);
		}

		public void Error(string msg)
		{
			Console.ForegroundColor = ErrorColor;
			_Error(Tag + msg);
		}
	}
}