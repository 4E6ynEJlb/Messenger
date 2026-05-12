using Application.Models.Internal.Constants;

namespace Application.Models.Input
{
    /// <summary>
    /// Model for messages resending
    /// </summary>
    public record ResendMessagesModel
    {
        /// <summary>
        /// Destination chat id
        /// </summary>
        public required Guid ChatId { get; init; }
        public required ChatType SourceChatType { get; init; }
        public required Guid SourceChatId { get; init; }
        /// <summary>
        /// Messages ids for resending. All messages should be from the same source chat
        /// </summary>
        public required Guid[] Messages { get; init; }
    }
}
