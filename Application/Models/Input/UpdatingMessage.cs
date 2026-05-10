namespace Application.Models.Input
{
    public record UpdatingMessage
    {
        public required Guid MessageId { get; init; }
        public required Guid ChatId { get; init; }
        public required string? MessageText { get; init; }
    }
}
