using NServiceBus;
using System;
using System.Collections.Generic;

namespace MessageBroker
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new BrokerServer();
            server.Start();
        }
    }
}
