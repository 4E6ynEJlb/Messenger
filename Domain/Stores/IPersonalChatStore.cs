using Domain.Models.Types;

namespace Domain.Stores
{
    public interface IPersonalChatStore
    {
        public Task<ChatInformation> GetChatShortInfoAsync(Guid chatId, Guid userId);///////////impl
        public Task<Message[]> GetMessagesAsync(Guid chatId, Guid gettingBy, uint messagesCount, DateTime sentBefore);
        public Task<Message> GetMessageAsync(Guid chatId, Guid messageId, Guid userId);///////////impl
        public Task<Guid> GetUserIdByChatIdAsync(Guid chatId, Guid gettingBy);
        public Task<Guid> CreateChatAsync(Guid userId1, Guid userId2);
        public Task<Guid> SendMessageAsync(Guid chatId, Guid senderId, string text, MediaFile[]? attachments);///////////impl
        public Task UpdateMessageTextAsync(Guid chatId, Guid messageId, Guid senderId, string newText);
        public Task BlockUserAsync(Guid blockingBy, Guid userId);///////////impl
        public Task UnblockUserAsync(Guid unblockingBy, Guid userId);///////////impl
        public Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid deletingBy);
        public Task DeleteFileFromMessageAsync(Guid chatId, Guid attachmentId, Guid deletingBy);
        public Task DeleteChatAsync(Guid chatId, Guid deletingBy);///////////rmk
    }
}
