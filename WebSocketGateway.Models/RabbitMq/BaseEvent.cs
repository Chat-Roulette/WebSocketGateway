namespace WebSocketGateway.Models.RabbitMq
{
    public abstract class BaseEvent
    {
        public string EventType { get; set; }
    }
}
