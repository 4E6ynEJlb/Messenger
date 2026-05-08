using System.Net;

namespace Domain.Models.Types
{
    public record BotConnectionLogRecord
    {
        public required IPAddress IPAddress { get; init; }
        public required DateTime ConnectedAt { get; init; }
        public required uint TokenVersion { get; init; }
    }
}
