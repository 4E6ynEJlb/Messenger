using Application.Models.Internal.Constants;

namespace UserAPI.Models
{
    public record UpdatePublicChatForm
    {
        /// <summary>
        /// if not null min length is 3 chars, max length is 64 chars. If null, chat name will not be updated
        /// </summary>
        public required string? NewChatName { get; init; }
        public required bool? NewSearchable { get; init; }
        /// <summary>
        /// False if chat image should not be updated, true if chat image should be updated.
        /// </summary>
        public required bool UpdateAvatar { get; init; }
        public required IFormFile? NewChatImage { get; init; }
        public required PublicChatMemberRole? NewDefaultMemberRole { get; init; }
    }
}
