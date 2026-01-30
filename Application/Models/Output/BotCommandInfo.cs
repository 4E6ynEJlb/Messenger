using Application.Models.Internal;

namespace Application.Models.Output
{
    public record BotCommandInfo
    {
        public required char? Prefix { get; init; }
        public required string Command { get; init; }
        public required string Description { get; init; }
        public required CommandArgument[] Arguments { get; init; }
    }
}
