namespace Application.Models.Input
{
    public record AddCommandModel
    {
        public required char Prefix { get; init; }
        public required string Command { get; init; }
        public required string Description { get; init; }
    }
}
