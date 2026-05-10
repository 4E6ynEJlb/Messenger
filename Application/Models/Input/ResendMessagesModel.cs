using Application.Models.Internal.Constants;

namespace Application.Models.Input
{
    public record ResendMessagesModel
    {
        public required Guid ChatId { get; init; }
        public required ChatType SourceChatType { get; init; }
        public required Guid SourceChatId { get; init; }
        public required Guid[] Messages { get; init; }
    }
}
