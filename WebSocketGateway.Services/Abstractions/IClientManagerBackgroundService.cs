using WebSocketGateway.Models;
using WebSocketGateway.Models.RabbitMq;

namespace WebSocketGateway.Services.Abstractions
{
    public interface IClientManagerBackgroundService
    {
        void AddClient(ClientModel client);
        Task NewMessageNotificationAsync(NewMessageEvent newMessageEvent);
        Task ValidateWebSocketsConnectionAsync();
    }
}
