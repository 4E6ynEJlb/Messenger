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

        public async Task NewBotMessage(Guid chatId, Guid messageId, Guid receiver)
        {
            await _updatesHub.Clients
                .User(receiver.ToString())
                .SendAsync("NewBotMessage", chatId, messageId);
        }

        public async Task NewPrivateMessage(Guid chatId, Guid messageId, Guid[] receivers)
        {
            await _updatesHub.Clients
                .Users(receivers.Select(r => r.ToString()).ToList())
                .SendAsync("NewPrivateMessage", chatId, messageId);
        }

        public async Task NewPublicMessage(Guid chatId, Guid messageId, Guid[] receivers)
        {
            await _updatesHub.Clients
                .Users(receivers.Select(r => r.ToString()).ToList())
                .SendAsync("NewPublicMessage", chatId, messageId);
        }

        public async Task PrivateMessagesRead(Guid chatId, Guid[] messagesId, Guid receiver)
        {
            await _updatesHub.Clients
                .User(receiver.ToString())
                .SendAsync("PrivateMessagesRead", chatId, messagesId);
        }

        public async Task PublicMessagesRead(Guid chatId, Guid userId, Guid[] messagesId, Guid receiver)
        {
            await _updatesHub.Clients
                .User(receiver.ToString())
                .SendAsync("PublicMessagesRead", chatId, userId, messagesId);
        }

        public async Task UserTypingPrivate(Guid chatId, Guid userId, Guid receiver)
        {
            await _updatesHub.Clients
                .User(receiver.ToString())
                .SendAsync("UserTypingPrivate", chatId, userId);
        }

        public async Task UserTypingPublic(Guid chatId, Guid userId, Guid[] receivers)
        {
            await _updatesHub.Clients
                .Users(receivers.Select(r => r.ToString()).ToList())
                .SendAsync("UserTypingPublic", chatId, userId);
        }
    }
}
