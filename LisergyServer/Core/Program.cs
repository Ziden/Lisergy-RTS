using LegendsServer.Core;
using System;

namespace LegendsServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new SocketServer();
            server.Start();

            Console.WriteLine("Server running");

            var ticks = new ServerTicks();
            ticks.StartTicks();

        }
    }
}
