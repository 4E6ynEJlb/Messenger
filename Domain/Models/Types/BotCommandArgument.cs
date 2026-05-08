namespace Domain.Models.Types
{
    public record BotCommandArgument
    {
        public required uint ArgumentId { get; init; }
        public required string Name { get; init; }
        public required string Type { get; init; }
    }
}
