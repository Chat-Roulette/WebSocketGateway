using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using WebSocketGateway.Models.RabbitMq;
using WebSocketGateway.Services.Abstractions;

namespace WebSocketGateway.Services.Implementations
{
    public class RabbitMqService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IClientManagerBackgroundService _clientManagerBackgroundService;

        public RabbitMqService(
            IConfiguration configuration,
            IClientManagerBackgroundService clientManagerBackgroundService)
        {
            _configuration = configuration;
            _clientManagerBackgroundService = clientManagerBackgroundService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _InitializeConnection();

            return Task.CompletedTask;
        }

        // [TODO] Not working on message receive
        private void _InitializeConnection()
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_configuration.GetSection("RabbitMq")["ConnectionUri"])
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "new_message_queue",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += async (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        Console.WriteLine(message);

                        var newMessageEvent = JsonSerializer.Deserialize<NewMessageEvent>(message);

                        await _clientManagerBackgroundService.NewMessageNotificationAsync(newMessageEvent);
                    };
                }
            }
        }
    }
}