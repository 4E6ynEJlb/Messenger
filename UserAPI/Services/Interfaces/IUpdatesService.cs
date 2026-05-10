using Application.Models.Internal.Constants;

namespace UserAPI.Services.Interfaces
{
    public interface IUpdatesService
    {
        public Task MessagesSent(Guid chatId, Guid[] messagesId, Guid[] userId, ChatType chatType, CancellationToken cancellationToken);

        public Task MessageUpdated(Guid chatId, Guid messageId, Guid[] userId, ChatType chatType, CancellationToken cancellationToken);

        public Task MessageDeleted(Guid chatId, Guid messageId, Guid[] userId, ChatType chatType, CancellationToken cancellationToken);

        public Task FileDeleted(Guid chatId, string file, Guid[] userId, ChatType chatType, CancellationToken cancellationToken);

        public Task ChatDeleted(Guid chatId, Guid[] userId, ChatType chatType, CancellationToken cancellationToken);

        public Task UserIsTyping(Guid chatId, Guid typingUserId, Guid[] destinationUserId, ChatType chatType, CancellationToken cancellationToken);
    }
}
