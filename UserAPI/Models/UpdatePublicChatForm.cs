using Application.Models;

namespace UserAPI.Models
{
    public record UpdatePublicChatForm
    {
        public required string? NewChatName { get; init; }
        public required bool? NewSearchable { get; init; }
        public required bool UpdateAvatar { get; init; }
        public required IFormFile? NewChatImage { get; init; }
        public required PublicChatMemberRole? NewDefaultMemberRole { get; init; }
    }
}
