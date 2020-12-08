using System;
using System.Collections.Generic;
using System.Text;

namespace LegendsServer.Core
{
    public class Log
    {
        public enum LogLevel
        {
            DEBUG,
            INFO,
        }

        public static void Debug(string msg)
        {
            Console.WriteLine(msg);
        }

        public static void Error(string msg, Exception e = null)
        {
            Console.WriteLine(msg);
            Console.WriteLine(e?.StackTrace);
        }

        public static void Info(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
