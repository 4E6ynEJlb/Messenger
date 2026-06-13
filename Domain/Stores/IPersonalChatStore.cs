using Domain.Models.Types;

namespace Domain.Stores
{
    public interface IPersonalChatStore
    {
        public Task<ChatInformation> GetChatShortInfoAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
        public Task<Message[]> GetMessagesAsync(Guid chatId, Guid gettingBy, uint messagesCount, DateTime sentBefore, CancellationToken cancellationToken);
        public Task<Message> GetMessageAsync(Guid chatId, Guid messageId, Guid userId, CancellationToken cancellationToken);
        public Task<Guid> GetUserIdByChatIdAsync(Guid chatId, Guid gettingBy, CancellationToken cancellationToken);
        public Task<Guid> GetMessageIdByMediaAsync(Guid chatId, Guid mediaId, CancellationToken cancellationToken);
        public Task<Guid> CreateChatAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken);
        public Task<bool> CheckMessageSendingAbilityAsync(Guid chatId, Guid senderId, Guid? replyTo, CancellationToken cancellationToken);
        public Task UpdateMessageTextAsync(Guid chatId, Guid messageId, Guid senderId, string? newText, CancellationToken cancellationToken);
        public Task BlockUserAsync(Guid blockingBy, Guid userId, CancellationToken cancellationToken);
        public Task UnblockUserAsync(Guid unblockingBy, Guid userId, CancellationToken cancellationToken);
        public Task DeleteChatAsync(Guid chatId, Guid deletingBy, CancellationToken cancellationToken);
    }
}
