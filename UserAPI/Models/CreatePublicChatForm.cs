using Application.Models;

namespace UserAPI.Models
{
    public record CreatePublicChatForm
    {
        public required string ChatName { get; init; }
        public required bool Searchable { get; init; }
        public required IFormFile? ChatImage { get; init; }
        public required PublicChatMemberRole DefaultMemberRole { get; init; }
    }
}
