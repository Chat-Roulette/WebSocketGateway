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
        private IConnection _connection;
        private IModel _channel;

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

        private void _InitializeConnection()
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_configuration.GetSection("RabbitMq")["ConnectionUri"])
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("new_message_notification", ExchangeType.Fanout);

            var queueName = _channel.QueueDeclare().QueueName;

            _channel.QueueBind(queue: queueName,
                                      exchange: "new_message_notification",
                                      routingKey: "");


            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += _handleReceivedAsync;

            _channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        private void _handleReceivedAsync(object? model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine(message);

            var newMessageEvent = JsonSerializer.Deserialize<NewMessageEvent>(message);

            _clientManagerBackgroundService.NewMessageNotificationAsync(newMessageEvent);
        }
    }
}