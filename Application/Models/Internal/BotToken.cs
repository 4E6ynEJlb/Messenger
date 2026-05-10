namespace Application.Models.Internal
{
    public record BotToken
    {
        public required byte[] TokenHash { get; init; }
        public required uint TokenVersion { get; init; }
    }
}
