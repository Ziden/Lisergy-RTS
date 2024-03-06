using System;
using System.Diagnostics;

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
            _Debug(Tag + msg);
        }

        public void Info(string msg) { _Info(Tag + msg); }
        public void Error(string msg) { _Error(Tag + msg); }

        public Action<string> _Debug = Console.WriteLine;
        public Action<string> _Info = Console.WriteLine;
        public Action<string> _Error = Console.Error.WriteLine;
    }
}
