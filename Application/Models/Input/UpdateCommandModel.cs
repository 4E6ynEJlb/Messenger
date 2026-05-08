namespace Application.Models.Input
{
    public record UpdateCommandModel
    {
        public required uint Id { get; init; }
        public required char? NewPrefix { get; init; }
        public required string? NewCommand { get; init; }
        public required string? NewDescription { get; init; }
    }
}
