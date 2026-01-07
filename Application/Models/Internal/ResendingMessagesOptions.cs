namespace Application.Models.Internal
{
    public record ResendingMessagesOptions
    {
        public required Guid[] MessagesId { get; init; }
        public required Guid SourceChatId { get; init; }
        public required Guid TargetChatId { get; init; }
    }
}
