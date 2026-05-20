using Domain.Models.Types;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Output
{
    public record PublicChatFullInfo
    {
        public PublicChatFullInfo() { }
        [SetsRequiredMembers]
        public PublicChatFullInfo(PublicChatFullInformation chat, string mediaPrefix)
        {
            ChatId = chat.ChatId;
            ChatImage = $"{mediaPrefix}/{chat.Avatar}";
            ChatName = chat.ChatName;
            Members = chat.Members.Select(m => new PublicChatMemberInfo(m, mediaPrefix)).ToArray();
        }
        public required Guid ChatId { get; init; }
        public required string ChatName { get; init; }
        public required string? ChatImage { get; init; }
        public required PublicChatMemberInfo[] Members { get; init; }
    }
}
