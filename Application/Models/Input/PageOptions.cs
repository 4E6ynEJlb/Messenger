namespace Application.Models.Input
{
    public record PageOptions
    {
        public required uint Page { get; init; }
        public required uint PageSize { get; init; }
    }
}
