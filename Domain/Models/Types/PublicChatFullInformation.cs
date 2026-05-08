namespace Domain.Models.Types
{
    public record PublicChatFullInformation
    {
        public required string ChatName { get; init; }
        public required Guid? Avatar { get; init; }
        public required ChatMemberInfo[] Members { get; init; }
    }
}
