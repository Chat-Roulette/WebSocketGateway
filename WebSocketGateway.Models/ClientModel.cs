using System.Net.WebSockets;

namespace WebSocketGateway.Models
{
    public class ClientModel
    {
        public Guid ClientId { get; set; }
        public WebSocket WebSocket { get; set; }
    }
}
