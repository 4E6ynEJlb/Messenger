using Application.Models.Internal.Constants;
using Application.Models.OptionsAndHelpers;
using Domain.Models.Types;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Output
{
    /// <summary>
    /// Short information about a chat for chat list
    /// </summary>
    public record ChatShortInfo
    {
        public ChatShortInfo() { }
        [SetsRequiredMembers]
        public ChatShortInfo(ChatInformation chatInformation, string mediaPrefix)
        {
            ChatId = chatInformation.ChatId;
            ChatType = ChatTypeConverter.Convert(chatInformation.ChatType);
            ChatName = chatInformation.ChatName;
            NewMessagesCount = chatInformation.NewMessagesCount;
            ChatImage = chatInformation.ChatImage is not null ? $"{mediaPrefix}/{chatInformation.ChatImage}" : null;
        }
        public required Guid ChatId { get; init; }
        public required ChatType ChatType { get; init; }
        public required string ChatName { get; init; }
        /// <summary>
        /// Count of messages that were sent to the chat after the user last opened it.
        /// </summary>
        public required int NewMessagesCount { get; init; }
        public required string? ChatImage { get; init; }
    }
}
