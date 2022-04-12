using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;
using WebSocketGateway.Models;
using WebSocketGateway.Services.Abstractions;
using WebSocketGateway.Services.Abstractions.External;

namespace WebSocketGateway.Services.Implementations
{
    public class ClientManagerBackgroundService : IClientManagerBackgroundService
    {
        private readonly Dictionary<Guid, IList<ClientModel>> _clients;
        private readonly IServiceProvider _serviceProvider;

        public ClientManagerBackgroundService(IServiceProvider serviceProvider)
        {
            _clients = new Dictionary<Guid, IList<ClientModel>>();
            _serviceProvider = serviceProvider;
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
            IServiceScope scope = _serviceProvider.CreateScope();

            IActivityService activityService = scope.ServiceProvider.GetRequiredService<IActivityService>();

            Console.WriteLine(activityService.GetHashCode());

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
                        await activityService.PingClientAsync(clientKey);
                    }
                }

                if (!clientConnections.Any())
                {
                    _clients.Remove(clientKey);
                }
            }

            scope.Dispose();
        }
    }
}
