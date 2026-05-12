using Application.Models.Internal;
using Application.Models.Internal.Constants;

namespace Application.Models.Input
{
    /// <summary>
    /// Model of public chat options
    /// </summary>
    public record PublicChatOptions
    {
        public required string ChatName { get; init; }
        /// <summary>
        /// If false then chat will not be visible in search results and users 
        /// will only be able to join it by invite link. If true then chat will 
        /// be visible in search results and users will be able to join it 
        /// without invite link.
        /// </summary>
        public required bool Searchable { get; init; }
        public required string? ChatImage { get; init; }
        /// <summary>
        /// Role that will be applied to newcome chat members
        /// </summary>
        public required PublicChatMemberRole DefaultMemberRole { get; init; }
    }
}
