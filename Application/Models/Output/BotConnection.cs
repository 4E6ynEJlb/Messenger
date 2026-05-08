using System.Net;

namespace Application.Models.Output
{
    public record BotConnection
    {
        public required IPAddress IPAddress { get; init; }
        public required DateTime ConnectedAt { get; init; }
        public required uint TokenVersion { get; init; }
    }
}
