using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBroker
{
    internal class BrokerServer
    {
        private List<IEndpointInstance> _endPoints = new List<IEndpointInstance>();

        internal void Start()
        {
            AppDomain.CurrentDomain.ProcessExit += Shutdown;
            Setup();
            Console.WriteLine("Running Message Broker");
            Console.ReadLine();
        }

        async internal void Setup()
        {
            var endpointConfiguration = new EndpointConfiguration("Battles");
            endpointConfiguration.UseTransport<LearningTransport>();
            var startableEndpoint = await Endpoint.Create(endpointConfiguration)
                .ConfigureAwait(false);
            var endpointInstance = await startableEndpoint.Start()
                .ConfigureAwait(false);
            _endPoints.Add(endpointInstance);
        }

        async internal void Shutdown(object sender, EventArgs e)
        {
            foreach (var endpoint in _endPoints)
                await endpoint.Stop().ConfigureAwait(false);
        }
    }
}
