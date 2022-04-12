using WebSocketGateway.Services.Abstractions.External;

namespace WebSocketGateway.Services.Implementations.External
{
    public class ActivityServiceHttp : IActivityService
    {
        private readonly HttpClient _httpClient;

        public ActivityServiceHttp(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task PingClientAsync(Guid clientId)
        {
            throw new NotImplementedException();
        }
    }
}
