namespace Domain.Models.Types
{
    public record ChatMemberInfo
    {
        public required Guid UserId { get; init; }
        public required string FullName { get; init; }
        public required Guid? Avatar { get; init; }
        public required EnPublicChatMemberRole Role { get; init; }
    }
}
