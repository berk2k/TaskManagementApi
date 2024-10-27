namespace TaskManagementApi.Services.Interfaces
{
    public interface IMessageQueueService
    {
        Task PublishMessageAsync<T>(T message);
    }
}
