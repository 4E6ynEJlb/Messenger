using Application.Models.Internal.Constants;

namespace Application.Models.Internal.Messages
{
    public record UserIsTypingMessage : BusMessage
    {
        public required Guid ChatId { get; init; }
        public required Guid TypingUserId { get; init; }
        public required Guid[] DestinationUserId { get; init; }
        public required ChatType ChatType { get; init; }
    }
}
