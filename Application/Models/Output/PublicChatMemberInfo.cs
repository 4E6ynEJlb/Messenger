using Application.Models.Internal.Constants;

namespace Application.Models.Output
{
    public record PublicChatMemberInfo
    {
        public required Guid UserId { get; init; }
        public required string FullName { get; init; }
        public required string? Avatar { get; init; }
        /// <summary>
        /// User role in public chat
        /// </summary>
        public required PublicChatMemberRole Role { get; init; }
    }
}
