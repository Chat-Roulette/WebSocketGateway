using WebSocketGateway.Services.Abstractions.External;

namespace WebSocketGateway.Services.Implementations.External
{
    public class ClientService : IClientService
    {
        public Task<bool> ValidateTokenAsync(string token)
        {
            throw new NotImplementedException();
        }
    }
}
