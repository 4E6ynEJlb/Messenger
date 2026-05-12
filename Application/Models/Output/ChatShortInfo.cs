using Application.Models.Internal.Constants;

namespace Application.Models.Output
{
    /// <summary>
    /// Short information about a chat for chat list
    /// </summary>
    public record ChatShortInfo
    {
        public required Guid ChatId { get; init; }
        public required ChatType ChatType { get; init; }
        public required string ChatName { get; init; }
        /// <summary>
        /// Count of messages that were sent to the chat after the user last opened it.
        /// </summary>
        public required int NewMessagesCount { get; init; }
        public required string? ChatImage { get; init; }
    }
}
