namespace WebSocketGateway.Models.RabbitMq
{
    public class NewMessageEvent : BaseEvent
    {
        public string Text { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid ConversationId { get; set; }
        public DateTime DateTime { get; set; }
    }
}
