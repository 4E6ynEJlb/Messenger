namespace Domain.Models.Types
{
    public record BotTokenInfo
    {
        public required byte[] TokenHash { get; init; }
        public required uint TokenVersion { get; init; }
    }
}
