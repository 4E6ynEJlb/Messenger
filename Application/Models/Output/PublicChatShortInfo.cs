using Application.Models.Internal.Constants;

namespace Application.Models.Output
{
    public record PublicChatShortInfo
    {
        public required Guid ChatId { get; init; }
        public required string ChatName { get; init; }
        public required string? ChatImage { get; init; }
        public required uint MembersCount { get; init; }
    }
}
