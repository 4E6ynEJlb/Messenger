namespace Application.Models.Input
{
    public record AddArgumentModel
    {
        public required uint CommandId { get; init; }
        public required string ArgumentName { get; init; }
        public required string ArgumentType { get; init; }
    }
}
