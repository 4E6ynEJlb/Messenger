using Application.Models.Internal.Constants;

namespace UserAPI.Models
{
    public record UpdatePublicChatForm
    {
        /// <summary>
        /// if not null min length is 3 chars, max length is 64 chars. If null, chat name will not be updated
        /// </summary>
        public string? NewChatName { get; init; }
        public bool? NewSearchable { get; init; }
        /// <summary>
        /// False if chat image should not be updated, true if chat image should be updated.
        /// </summary>
        public required bool UpdateAvatar { get; init; }
        public IFormFile? NewChatImage { get; init; }
        public PublicChatMemberRole? NewDefaultMemberRole { get; init; }
    }
}
