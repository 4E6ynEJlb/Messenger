using Application.Models.Internal.Constants;
using Domain.Models.Types;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Output
{
    public record PublicChatShortInfo
    {
        public PublicChatShortInfo() { }
        [SetsRequiredMembers]
        public PublicChatShortInfo(PublicChatFullInformation c, string mediaPrefix)
        {
            ChatId = c.ChatId;
            ChatName = c.ChatName;
            ChatImage = c.Avatar is not null ? $"{mediaPrefix}/{c.Avatar}" : null;
            MembersCount = (uint)c.Members.Count();
        }
        public required Guid ChatId { get; init; }
        public required string ChatName { get; init; }
        public required string? ChatImage { get; init; }
        public required uint MembersCount { get; init; }
    }
}
