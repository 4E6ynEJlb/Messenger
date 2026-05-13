using Application.Models.Internal.Constants;

namespace Application.Models.Internal.Messages
{
    public record FileDeletedMessage : BusMessage
    {
        public required Guid ChatId { get; init; }
        public required string File { get; init; }
        public required Guid MessageId { get; init; }
        public required Guid[] UserId { get; init; }
        public required ChatType ChatType { get; init; }
    }
}
