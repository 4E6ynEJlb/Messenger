using Application.Models.Internal.Constants;

namespace Application.Models.Internal.Messages
{
    public record ChatDeletedMessage : BusMessage
    {
        public required Guid ChatId { get; init; }
        public required Guid[] UserId { get; init; }
        public required ChatType ChatType { get; init; }
    }
}
