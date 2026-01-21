namespace Application.Models.Output
{
    public record PublicChatFullInfo
    {
        public required Guid ChatId { get; init; }
        public required string ChatName { get; init; }
        public required string? ChatImage { get; init; }
        public required bool IsSearchable { get; init; }
        public required PublicChatMemberRole DefaultMemberRole { get; init; }
        public required PublicChatMemberInfo[] Members { get; init; }
    }
}
