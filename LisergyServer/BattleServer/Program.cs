using BattleServer.Core;
using Game.Events;
using LisergyMessageQueue;
using System;

namespace BattleServer
{
    public class Program
    {
        public static int PORT = 666;

        static void Main(string[] args)
        {
            Initialize();
          
        }

        public static string Path()
        {
            string folder = Environment.CurrentDirectory;
            return folder;
        }

        public static void Initialize()
        {
            Console.WriteLine("=============================");
            Console.WriteLine("=     BATTLE SERVER         =");
            Console.WriteLine("=============================");

            /*
            var battleServer = new BattleServer(PORT);
            LisergyMQ.StartListening(MessageQueues.PENDING_BATTLES);
            battleServer.RunServer();
            */
        }
    }
}
