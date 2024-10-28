using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TaskManagementApi.Models;

namespace TaskManagementApi.Services
{
    public class RabbitMqConsumerService
    {
        private readonly IConnection _connection;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _consumeTask;

        public RabbitMqConsumerService(IConnection connection)
        {
            _connection = connection;
        }

        public void StartConsuming()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _consumeTask = Task.Run(() => ConsumeMessages(_cancellationTokenSource.Token));
        }

        private void ConsumeMessages(CancellationToken cancellationToken)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    // Pass the raw JSON message directly
                    await HandleMessageAsync(message);
                };

                channel.BasicConsume(queue: "task_queue", autoAck: true, consumer: consumer);

                while (!cancellationToken.IsCancellationRequested)
                {
                    // Keep the task alive until cancellation is requested
                    Thread.Sleep(100);
                }
            }
        }


        public void StopConsuming()
        {
            _cancellationTokenSource?.Cancel();
            _consumeTask?.Wait();
        }

        private Task HandleMessageAsync(string message)
        {
            // Log or process the raw message directly
            Console.WriteLine($"Received Message: {message}");
            return Task.CompletedTask;
        }

    }
}
