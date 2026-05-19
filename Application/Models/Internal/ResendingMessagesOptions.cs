using Application.Models.Internal.Constants;

namespace Application.Models.Internal
{
    public record ResendingMessagesOptions
    {
        public required Guid ChatId { get; init; }
        public required Guid[] MessagesId { get; init; }
        public required Guid SourceChatId { get; init; }
        public required ChatType SourceChatType { get; init; }
        public required Guid Author { get; init; }
    }
}
