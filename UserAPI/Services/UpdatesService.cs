using Application.Models.Internal.Constants;
using Microsoft.AspNetCore.SignalR;
using UserAPI.Hubs;
using UserAPI.Services.Interfaces;

namespace UserAPI.Services
{
    public class UpdatesService : IUpdatesService
    {
        private readonly UpdatesHub _updatesHub;
        public UpdatesService(UpdatesHub updatesHub)
        {
            _updatesHub = updatesHub;
        }

        public async Task MessagesSent(Guid chatId, Guid[] messagesId, Guid[] userId, ChatType chatType, CancellationToken cancellationToken)
        {
            await _updatesHub.Clients.Users(userId.Select(u => u.ToString()).ToArray()).SendAsync("MessageSent", chatId, messagesId, chatType, cancellationToken);
        }

        public async Task MessageUpdated(Guid chatId, Guid messageId, Guid[] userId, ChatType chatType, CancellationToken cancellationToken)
        {
            await _updatesHub.Clients.Users(userId.Select(u => u.ToString()).ToArray()).SendAsync("MessageUpdated", chatId, messageId, chatType, cancellationToken);
        }

        public async Task MessageDeleted(Guid chatId, Guid messageId, Guid[] userId, ChatType chatType, CancellationToken cancellationToken)
        {
            await _updatesHub.Clients.Users(userId.Select(u => u.ToString()).ToArray()).SendAsync("MessageDeleted", chatId, messageId, chatType, cancellationToken);
        }
        
        public async Task FileDeleted(Guid chatId, string file, Guid messageId, Guid[] userId, ChatType chatType, CancellationToken cancellationToken)
        {
            await _updatesHub.Clients.Users(userId.Select(u => u.ToString()).ToArray()).SendAsync("FileDeleted", chatId, file, messageId, chatType, cancellationToken);
        }

        public async Task ChatDeleted(Guid chatId, Guid[] userId, ChatType chatType, CancellationToken cancellationToken)
        {
            await _updatesHub.Clients.Users(userId.Select(u => u.ToString()).ToArray()).SendAsync("ChatDeleted", chatId, chatType, cancellationToken);
        }

        public async Task UserIsTyping(Guid chatId, Guid typingUserId, Guid[] destinationUserId, ChatType chatType, CancellationToken cancellationToken)
        {
            await _updatesHub.Clients.Users(destinationUserId.Select(u => u.ToString()).ToArray()).SendAsync("UserIsTyping", chatId, typingUserId, chatType, cancellationToken);
        }

        public async Task BotButtonsUpdated(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            await _updatesHub.Clients.User(userId.ToString()).SendAsync("BotButtonsUpdated", chatId, cancellationToken);
        }
    }
}
