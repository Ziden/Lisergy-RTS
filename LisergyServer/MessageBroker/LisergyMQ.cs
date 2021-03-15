using System;
using RabbitMQ.Client;
using Game.Events;
using RabbitMQ.Client.Events;
using LisergyServer.Core;

namespace LisergyMessageQueue
{
    public class LisergyMQ
    {
        private static string HOST = "localhost";
        private static IModel _channel;

        public static IModel Channel
        {
            get
            {
                if (_channel == null)
                {
                    var factory = new ConnectionFactory() { HostName = HOST };
                    var connection = factory.CreateConnection();
                    _channel = connection.CreateModel();
                }
                return _channel;
            }
        }

        private static string GetQueueName(MessageQueues queueType)
        {
            return $"q_{queueType}";
        }

        public static void Send<Ev>(MessageQueues queue, Ev message) where Ev : ServerEvent
        {

            var queueName = GetQueueName(queue);
            Channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            var body = Serialization.FromEvent<Ev>(message);
            var properties = Channel.CreateBasicProperties();
            properties.Persistent = true;
            Channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: properties,
                                     body: body);
            Console.WriteLine("[LMQ] Sent {0}", message.GetType().Name);

        }

        public static void StartListening(MessageQueues queue)
        {

            var queueName = GetQueueName(queue);
            Channel.QueueDeclare(queue: queueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                Console.WriteLine("[LMQ] Received event");
                EventEmitter.CallEventFromBytes(null, body);
            };
            Channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);
            Console.WriteLine($"[LMQ] Registered listener for {queue}");
        }
    }
}

