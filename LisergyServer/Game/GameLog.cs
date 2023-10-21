using System;
using System.Diagnostics;

namespace Game
{
    public interface IGameLog
    {
        public void Debug(string msg);
        public void Info(string msg);
        public void Error(string msg);
    }
    public class GameLog : IGameLog
    {
        private string _tag;

        public GameLog(string tag)
        {
            _tag = tag+" ";
        }

        public void Debug(string msg)
        {
            _Debug(_tag + msg);
        }

        public void Info(string msg) { _Info(_tag + msg); }
        public void Error(string msg) { _Error(_tag + msg); }

        public Action<string> _Debug = Console.WriteLine;
        public Action<string> _Info = Console.WriteLine;
        public Action<string> _Error = Console.Error.WriteLine;
    }
}
