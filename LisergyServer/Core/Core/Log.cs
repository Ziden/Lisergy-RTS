using System;
using System.Collections.Generic;
using System.Text;

namespace LegendsServer.Core
{
    public class Log
    {
        public static Action<string> Debug = Console.WriteLine;
        public static Action<string> Info = Console.WriteLine;
        public static Action<string> Error = Console.Error.WriteLine;
    }
}
