using Application.Models.Internal.Constants;

namespace UserAPI.Services.Interfaces
{
    public interface IUpdatesService
    {
        public Task MessageSent(Guid chatId, Guid messageId, Guid[] userId, ChatType chatType, CancellationToken cancellationToken);

        public Task MessageUpdated(Guid chatId, Guid messageId, Guid[] userId, ChatType chatType, CancellationToken cancellationToken);

        public Task MessageDeleted(Guid chatId, Guid messageId, Guid[] userId, ChatType chatType, CancellationToken cancellationToken);

        public Task UserIsTyping(Guid chatId, Guid typingUserId, Guid[] destinationUserId, ChatType chatType, CancellationToken cancellationToken);
    }
}
