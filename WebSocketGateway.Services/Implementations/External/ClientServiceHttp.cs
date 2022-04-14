using System.Net;
using System.Text.Json;
using WebSocketGateway.Services.Abstractions.External;
using WebSocketGateway.Services.Configuration;

namespace WebSocketGateway.Services.Implementations.External
{
    public class ClientServiceHttp : IClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IClientServiceHttpConfiguration _serviceConfiguration;

        public ClientServiceHttp(
            HttpClient httpClient,
            IClientServiceHttpConfiguration serviceConfiguration)
        {
            _httpClient = httpClient;
            _serviceConfiguration = serviceConfiguration;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var baseUri = new Uri(_serviceConfiguration.Url);
            var uri = new Uri(baseUri, "/api/client/token/validate");

            var tokenJson = JsonSerializer.Serialize(new { Token = token });
            var tokenContent = new StringContent(tokenJson);

            var response = await _httpClient.PostAsync(uri.ToString(), tokenContent);

            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
