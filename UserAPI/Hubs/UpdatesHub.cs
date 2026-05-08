using Application.Models.Internal.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using UserAPI.Models;

namespace UserAPI.Hubs
{
    [Authorize(Policy = Policies.USER_POLICY)]
    public class UpdatesHub:Hub
    {        
        public async Task MessageSent(Guid chatId, Guid messageId, Guid[] userId, ChatType chatType)
        {
            await Clients.Users(userId.Select(u => u.ToString()).ToArray()).SendAsync("MessageSent", chatId, messageId, chatType);
        }

        public async Task MessageUpdated(Guid chatId, Guid messageId, Guid[] userId, ChatType chatType)
        {
            await Clients.Users(userId.Select(u => u.ToString()).ToArray()).SendAsync("MessageUpdated", chatId, messageId, chatType);
        }

        public async Task MessageDeleted(Guid chatId, Guid messageId, Guid[] userId, ChatType chatType)
        {
            await Clients.Users(userId.Select(u => u.ToString()).ToArray()).SendAsync("MessageDeleted", chatId, messageId, chatType);
        }

        public async Task UserIsTyping(Guid chatId, Guid typingUserId, Guid[] destinationUserId, ChatType chatType)
        {
            await Clients.Users(destinationUserId.Select(u => u.ToString()).ToArray()).SendAsync("UserIsTyping", chatId, typingUserId, chatType);
        }
    }
}
