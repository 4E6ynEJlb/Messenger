using Application.Models.Internal.Constants;

namespace Application.Models.Output
{
    public record ChatShortInfo
    {
        public required Guid ChatId { get; init; }
        public required ChatType ChatType { get; init; }
        public required string ChatName { get; init; }
        public required int NewMessagesCount { get; init; }
        public required string? ChatImage { get; init; }
    }
}
