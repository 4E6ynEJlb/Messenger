namespace Domain.Models.Types
{
    public record PublicChatOptions
    {
        public required string ChatName { get; init; }
        public required bool IsSearchable { get; init; }
        public required Guid? Avatar { get; init; }
        public required EnPublicChatMemberRole DefaultMemberRole { get; init; }
    }
}
