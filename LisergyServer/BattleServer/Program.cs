using LisergyMessageQueue;
using System;

namespace BattleService
{
    class Program
    {
        private static BattleServer _server;

        // docker run -d --hostname my-rabbit --name ecomm-rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management

        static void Main(string[] args)
        {
            Console.WriteLine("--- Starting Battle Server ---");
            _server = new BattleServer();
            _server.StartListening();
            Console.ReadLine();
        }
    }
}
