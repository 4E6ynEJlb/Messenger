namespace Application.Models.Output
{
    public record PublicChatMemberInfo
    {
        public required Guid UserId { get; init; }
        public required string FullName { get; init; }
        public required string? Avatar { get; init; }
        public required PublicChatMemberRole Role { get; init; }
    }
}
