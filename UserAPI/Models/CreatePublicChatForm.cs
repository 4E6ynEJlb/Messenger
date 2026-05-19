using Application.Models.Internal.Constants;

namespace UserAPI.Models
{
    public record CreatePublicChatForm
    {
        /// <summary>
        /// Min length is 3 chars and max length is 64 chars
        /// </summary>
        public required string ChatName { get; init; }
        /// <summary>
        /// If true then chat will be visible in search results
        /// </summary>
        public required bool Searchable { get; init; }
        public IFormFile? ChatImage { get; init; }
        /// <summary>
        /// Role that will be applied to newcome chat members
        /// </summary>
        public required PublicChatMemberRole DefaultMemberRole { get; init; }
    }
}
