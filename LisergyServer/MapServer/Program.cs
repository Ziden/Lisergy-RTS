using Game;
using Game.Events;
using GameDataTest;
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

            var server = new MapSocketServer(PORT);
           
            server.RunServer();
        }
    }
}
