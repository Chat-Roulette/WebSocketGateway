namespace WebSocketGateway.Services.Abstractions.External
{
    public interface IActivityService
    {
        Task PingClientAsync(Guid clientId);
    }
}
