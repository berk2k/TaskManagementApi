using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManagementApi.Services.Interfaces;

namespace TaskManagementApi.Services
{
    public class MessageQueueService
    {
        private readonly IConnection _connection;
        private IModel _channel;

        private const string exchangeName = "task_exchange";
        private const string queueName = "task_queue";
        private const string routingKey = "task_routing_key";

        public MessageQueueService(IConnection connection)
        {
            _connection = connection;
            _channel = CreateOrGetChannel();
            InitializeQueueAndExchange();
        }

        private void InitializeQueueAndExchange()
        {


            
            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

            // declare queue (if not exists)
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            // bind queue with exchange
            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);
        }

       
        public void DeclareQueue(string queueName)
        {
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        
        public void DeclareExchange(string exchangeName, string exchangeType)
        {
            _channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType);
        }

        
        public void BindQueue(string queueName, string exchangeName, string routingKey)
        {
            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);
        }

        
        public async Task PublishMessageAsync<T>(T message)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
        }

       
        public string GetExchangeName()
        {
            return exchangeName;
        }

       
        public string GetRoutingKey()
        {
            return routingKey;
        }

        
        private IModel CreateOrGetChannel()
        {
            return _channel ?? (_channel = _connection.CreateModel());
        }

       
        public void CloseConnection()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
