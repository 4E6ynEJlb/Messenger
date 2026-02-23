namespace Domain.Models.Types
{
    public record ChatInformation
    {
        public required Guid ChatId { get; init; }
        public required string ChatName { get; init; }
        public required int NewMessagesCount { get; init; }
        public required Guid? ChatImage { get; init; }
    }
}
