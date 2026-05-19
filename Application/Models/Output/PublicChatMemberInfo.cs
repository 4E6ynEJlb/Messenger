using Application.Models.Helpers;
using Application.Models.Internal.Constants;
using Domain.Models.Types;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Output
{
    public record PublicChatMemberInfo
    {
        public PublicChatMemberInfo() { }
        [SetsRequiredMembers]
        public PublicChatMemberInfo(ChatMemberInfo member, string mediaPrefix)
        {
            UserId = member.UserId;
            FullName = member.FullName;
            Role = ChatRoleConverter.Convert(member.Role);
            Avatar = $"{mediaPrefix}/{member.Avatar}";
        }
        public required Guid UserId { get; init; }
        public required string FullName { get; init; }
        public required string? Avatar { get; init; }
        /// <summary>
        /// User role in public chat
        /// </summary>
        public required PublicChatMemberRole Role { get; init; }
    }
}
