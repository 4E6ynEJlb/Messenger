namespace Application.Models.Input
{
    public record UpdateArgumentModel
    {
        public required uint CommandId { get; init; }
        public required uint ArgumentId { get; init; }
        public required string? NewArgumentName { get; init; }
        public required string? NewArgumentType { get; init; }
    }
}
