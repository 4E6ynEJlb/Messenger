using Application.Models.Internal.Constants;

namespace Application.Models.Internal.Messages
{
    public record MessagesSentMessage : BusMessage
    {
        public required Guid ChatId { get; init; }
        public required Guid[] MessagesId { get; init; } 
        public required Guid[] UserId { get; init; }
        public required ChatType ChatType { get; init; }
    }
}
