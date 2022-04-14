using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebSocketGateway.Models;
using WebSocketGateway.Services.Abstractions;
using WebSocketGateway.Services.Abstractions.External;

namespace WebSocketGateway.Web.Controllers
{
    [ApiController]
    [Route("api/gateway")]
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
            if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var stringValuesToken))
            {
                return Unauthorized("Authorization header required");
            }

            var token = stringValuesToken.ToString();

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken;

            try
            {
                jwtSecurityToken = handler.ReadJwtToken(token);
            }
            catch (Exception)
            {
                return Unauthorized("Invalid Authorization header format");
            }

            if (!await _clientService.ValidateTokenAsync(token))
            {
                return Unauthorized();
            }

            var clientId = jwtSecurityToken.Claims.First((c) => c.Type == "clientId").Value;
            var clientIdGuid = Guid.Parse(clientId);

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
