using Application.Models;

namespace UserAPI.Models
{
    public record PublicChatOptionsForm
    {
        public required string ChatName { get; init; }
        public required bool Searchable { get; init; }
        public required IFormFile? ChatImage { get; init; }
        public required PublicChatMemberRole DefaultMemberRole { get; init; }
    }
}
