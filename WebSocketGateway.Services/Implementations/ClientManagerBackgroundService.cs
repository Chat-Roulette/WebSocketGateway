using System.Net.WebSockets;
using WebSocketGateway.Models;
using WebSocketGateway.Services.Abstractions;
using WebSocketGateway.Services.Abstractions.External;

namespace WebSocketGateway.Services.Implementations
{
    public class ClientManagerBackgroundService : IClientManagerBackgroundService
    {
        private readonly Dictionary<Guid, IList<ClientModel>> _clients;
        private readonly IActivityService _activityService;

        public ClientManagerBackgroundService(IActivityService activityService)
        {
            _clients = new Dictionary<Guid, IList<ClientModel>>();
            _activityService = activityService;
        }

        public void AddClient(ClientModel client)
        {
            _clients.TryGetValue(client.ClientId, out var clientModels);

            if (clientModels is null)
            {
                _clients.Add(client.ClientId, new ClientModel[] { client });
            }
            else
            {
                clientModels.Add(client);
            }
        }

        public async Task ValidateWebSocketsConnectionAsync()
        {
            foreach (var clientKey in _clients.Keys.ToArray())
            {
                var clientConnections = _clients[clientKey];

                foreach (var clientConnection in clientConnections.ToArray())
                {
                    if (clientConnection.WebSocket.State != WebSocketState.Open &&
                        clientConnection.WebSocket.State != WebSocketState.Connecting)
                    {
                        clientConnection.WebSocket.Abort();
                        clientConnections.Remove(clientConnection);
                    }
                    else
                    {
                        await _activityService.PingClientAsync(clientKey);
                    }
                }

                if (!clientConnections.Any())
                {
                    _clients.Remove(clientKey);
                }
            }
        }
    }
}
