namespace TaskManagementApi.Services
{
    public class RabbitMqBackgroundService : BackgroundService
    {
        private readonly RabbitMqConsumerService _consumer;

        public RabbitMqBackgroundService(RabbitMqConsumerService consumer)
        {
            _consumer = consumer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.StartConsuming(); // Start consuming messages
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _consumer.StopConsuming(); // Stop consuming messages
            return base.StopAsync(stoppingToken);
        }
    }
}
