using WebSocketGateway.Services.Abstractions.External;

namespace WebSocketGateway.Services.Implementations.External
{
    public class ActivityService : IActivityService
    {
        public Task PingClientAsync(Guid clientId)
        {
            throw new NotImplementedException();
        }
    }
}
