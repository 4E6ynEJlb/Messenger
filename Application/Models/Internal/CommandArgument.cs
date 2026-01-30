namespace Application.Models.Internal
{
    public record CommandArgument
    {
        public required string Name { get; init; }
        public required string Type { get; init; }
    }
}
