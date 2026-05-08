namespace Domain.Models.Types
{
    public record BotInfo
    {
        public required Guid BotId { get; init; }
        public required string Name { get; init; }
        public required string Tag { get; init; }
        public required string? Avatar { get; init; }
        public required string? Description { get; init; }
        public required bool IsEnabled { get; init; }
    }
}
