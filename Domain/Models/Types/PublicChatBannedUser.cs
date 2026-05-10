namespace Domain.Models.Types
{
    public record PublicChatBannedUser
    {
        public required Guid UserId { get; init; }
        public required Guid BannedBy { get; init; }
        public required DateTime BannedAt { get; init; }
    }
}
