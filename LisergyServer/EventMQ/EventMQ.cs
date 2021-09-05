using System;
using Game;
using Game.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LisergyMessageQueue
{
    public class EventMQ
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

        public static void Send<Ev>(string queue, Ev message) where Ev : BaseEvent
        {
            Channel.QueueDeclare(queue: queue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            var body = Serialization.FromEvent<Ev>(message);
            var properties = Channel.CreateBasicProperties();
            properties.Persistent = true;
            Channel.BasicPublish(exchange: "",
                                     routingKey: queue,
                                     basicProperties: properties,
                                     body: body);
            Log.Debug("[LMQ] Sent "+ message.GetType().Name+" to queue "+queue);
        }

        public static void StopListening(string queue)
        {
            Channel.QueueDelete(queue);
            Log.Debug("[LMQ] Destroyed queue "+queue);
        }

        public static void StartListening(string queue, Action<byte[]> ReceiveListener)
        {
            Channel.QueueDeclare(queue: queue,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                Console.WriteLine("[LMQ] Received event");
                ReceiveListener(body);
            };
            Channel.BasicConsume(queue: queue,
                                     autoAck: true,
                                     consumer: consumer);
            Log.Debug($"[LMQ] Registered listener for queue {queue}");
        }
    }
}
