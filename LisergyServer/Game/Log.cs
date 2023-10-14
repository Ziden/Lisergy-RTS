using System;
using System.Diagnostics;

namespace Game
{
    public class Log
    {
        public static void Debug(string msg)
        {
            _Debug(msg);
        }

        public static void Info(string msg) { _Info(msg); }
        public static void Error(string msg) { _Error(msg); }

        public static Action<string> _Debug = Console.WriteLine;
        public static Action<string> _Info = Console.WriteLine;
        public static Action<string> _Error = Console.Error.WriteLine;
    }
}
