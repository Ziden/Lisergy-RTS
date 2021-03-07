using System;

namespace LisergyServer.Core
{
    public static class Log
    {
        public static Action<string> Debug = Console.WriteLine;
        public static Action<string> Info = Console.WriteLine;
        public static Action<string> Error = Console.Error.WriteLine;
    }
}
