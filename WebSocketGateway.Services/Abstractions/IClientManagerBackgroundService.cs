using WebSocketGateway.Models;

namespace WebSocketGateway.Services.Abstractions
{
    public interface IClientManagerBackgroundService
    {
        void AddClient(ClientModel client);
        Task ValidateWebSocketsConnectionAsync();
    }
}
