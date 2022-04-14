using WebSocketGateway.Services.Abstractions.External;
using WebSocketGateway.Services.Configuration;

namespace WebSocketGateway.Services.Implementations.External
{
    public class ActivityServiceHttp : IActivityService
    {
        private readonly HttpClient _httpClient;
        private readonly IActivityServiceHttpConfiguration _serviceConfiguration;

        public ActivityServiceHttp(
            HttpClient httpClient,
            IActivityServiceHttpConfiguration serviceConfiguration)
        {
            _httpClient = httpClient;
            _serviceConfiguration = serviceConfiguration;
        }

        public async Task PingClientAsync(Guid clientId)
        {
            var baseUri = new Uri(_serviceConfiguration.Url);

            var uri = new Uri(baseUri, string.Format("/api/activity/client/{0}/ping", clientId));

            await _httpClient.PostAsync(uri.ToString(), null);
        }
    }
}
