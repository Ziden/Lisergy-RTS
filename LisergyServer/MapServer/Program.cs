using Game;
using Game.Events;
using GameDataTest;
using LisergyMessageQueue;
using LisergyServer.Core;
using System;
using System.Threading;

namespace MapServer
{
    public class Program
    {
        private static readonly int PORT = 1337;

        public static void Main(string[] args)
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
            Console.WriteLine("=        MAP SERVER         =");
            Console.WriteLine("=============================");
            Thread.Sleep(1000);
            var server = new MapService(PORT);
           
            server.RunServer();
        }
    }
}
