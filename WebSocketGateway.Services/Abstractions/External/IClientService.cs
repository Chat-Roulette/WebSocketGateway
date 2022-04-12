namespace WebSocketGateway.Services.Abstractions.External
{
    public interface IClientService
    {
        Task<bool> ValidateTokenAsync(string token);
    }
}
