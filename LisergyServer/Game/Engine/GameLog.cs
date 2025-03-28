using System;

namespace Game.Engine
{
    public interface IGameLog
    {
        public void Debug(string msg);
        public void Info(string msg);
        public void Error(string msg);
    }
    public class GameLog : IGameLog
    {
        public string Tag { get; private set; }

        public GameLog(string tag)
        {
            Tag = tag + " ";
        }

        public void Debug(string msg)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.White;
            _Debug(Tag + msg);
#endif
        }

        public void Info(string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            _Info(Tag + msg);
        }

        public void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            _Error(Tag + msg);
        }

        public Action<string> _Debug = Console.WriteLine;
        public Action<string> _Info = (msg) =>
        {
            Console.WriteLine(msg);
        };
        public Action<string> _Error = Console.Error.WriteLine;
    }
}
