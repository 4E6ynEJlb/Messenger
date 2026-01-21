using Application.Models.Internal;

namespace Application.Models.Input
{
    public record PublicChatOptions
    {
        public required string ChatName { get; init; }
        public required bool Searchable { get; init; }
        public required FileUpload? ChatImage { get; init; }
        public required Guid[] InitialMembers { get; init; }
        public required PublicChatMemberRole DefaultMemberRole { get; init; }
    }
}
