using Application.Models.Internal.Constants;
using Domain.Models.Types;

namespace Application.Models.OptionsAndHelpers
{
    internal static class ChatTypeConverter
    {
        internal static EnChatType Convert(ChatType chatType) => chatType switch
        {
            ChatType.Personal => EnChatType.Personal,
            ChatType.Group => EnChatType.Public,
            ChatType.Bot => EnChatType.Bot,
            _ => throw new Exception("Invalid chat type")
        };

        internal static ChatType Convert(EnChatType chatType) => chatType switch
        {
            EnChatType.Personal => ChatType.Personal,
            EnChatType.Public => ChatType.Group,
            EnChatType.Bot => ChatType.Bot,
            _ => throw new Exception("Invalid chat type")
        };
    }
}
