using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebSocketGateway.Models;
using WebSocketGateway.Services.Abstractions;
using WebSocketGateway.Services.Abstractions.External;

namespace WebSocketGateway.Web.Controllers
{
    [ApiController]
    [Route("gateway")]
    public class WebSocketController : ControllerBase
    {
        private readonly IClientManagerBackgroundService _clientManagerBackgoundService;
        private readonly IClientService _clientService;

        public WebSocketController(
            IClientManagerBackgroundService clientManagerBackgoundService,
            IClientService clientService)
        {
            _clientManagerBackgoundService = clientManagerBackgoundService;
            _clientService = clientService;
        }

        [HttpGet("ws")]
        public async Task<IActionResult> AcceptWebSocketConnection()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var clientId = jwtSecurityToken.Claims.First((c) => c.Type == "clientId")?.ToString();

            if (!Guid.TryParse(clientId, out var clientIdGuid) &&
                clientIdGuid != Guid.Empty &&
                !await _clientService.ValidateTokenAsync(token))
            {
                return Unauthorized();
            }

            var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();

            var client = new ClientModel
            {
                ClientId = clientIdGuid,
                WebSocket = ws
            };

            _clientManagerBackgoundService.AddClient(client);

            return Ok();
        }
    }
}
