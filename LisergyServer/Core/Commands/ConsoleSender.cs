using System;

namespace BaseServer.Commands
{
    public class ConsoleSender : CommandSender
    {
        public override void SendMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
