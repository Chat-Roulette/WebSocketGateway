using Quartz;
using WebSocketGateway.Services.Abstractions;

namespace WebSocketGateway.Services.Implementations
{
    public class ScheduledConnectionCheck : IJob
    {
        private readonly IClientManagerBackgroundService _clientManagerBackgroundService;

        public ScheduledConnectionCheck(IClientManagerBackgroundService clientManagerBackgroundService)
        {
            _clientManagerBackgroundService = clientManagerBackgroundService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _clientManagerBackgroundService.ValidateWebSocketsConnectionAsync();
        }
    }
}
