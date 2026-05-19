using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Output
{
    public record PublicChatBannedUser
    {
        public PublicChatBannedUser() { }
        [SetsRequiredMembers]
        public PublicChatBannedUser(Domain.Models.Types.PublicChatBannedUser bannedUser)
        {
            UserId = bannedUser.UserId;
            BannedBy = bannedUser.BannedBy;
            BannedAt = bannedUser.BannedAt;
        }
        public required Guid UserId { get; init; }
        public required Guid BannedBy { get; init; }
        public required DateTime BannedAt { get; init; }
    }
}
