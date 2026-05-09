namespace Domain.Models.Types
{
    public record BotCommandInfo
    {
        public required uint CommandId { get; init; }
        public required char Prefix { get; init; }
        public required string Command { get; init; }
        public string Description { get; init; } = "";
        public required BotCommandArgument[] Arguments { get; init; }
    }
}
