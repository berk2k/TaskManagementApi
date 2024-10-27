using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManagementApi.Services.Interfaces;


namespace TaskManagementApi.Services
{
    public class MessageQueueService : IMessageQueueService
    {
        private readonly IConnection _connection;

        public MessageQueueService(IConnection connection)
        {
            _connection = connection;
        }

        public async Task PublishMessageAsync<T>(T message)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(exchange: "", routingKey: "task_queue", basicProperties: null, body: body);
            }
        }
    }
}
