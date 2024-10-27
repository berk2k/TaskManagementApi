using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using TaskManagementApi.Context;
using TaskManagementApi.Services.Interfaces;
using TaskManagementApi.Services;
using DotNetEnv;


namespace TaskManagementApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // .env dosyas�n� y�kleme
            Env.Load();

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddSingleton<IConnection>(provider =>
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                return factory.CreateConnection();
            });

            builder.Services.AddScoped<IMessageQueueService, MessageQueueService>();
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // PostgreSQL ba�lant� dizesini .env dosyas�ndan alma
            var connectionString = $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};" +
                                   $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};" +
                                   $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
                                   $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}";

            // PostgreSQL DbContext ayarlar�n� ekleme
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            // RabbitMQ ba�lant�s� i�in yap�land�rma
            var factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME"),
                Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")
            };

            // RabbitMQ ba�lant�s�n� servis olarak ekleme
            builder.Services.AddSingleton(factory);

            var app = builder.Build();

            // HTTP request pipeline'� yap�land�rma
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
