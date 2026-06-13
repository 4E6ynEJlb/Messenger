using Domain.Models.Types;

namespace Domain.Stores
{
    public interface ISynchronizationStore
    {
        public Task DeletePublicMessageAsync(Guid chatId, Guid messageId, Guid deletingBy, CancellationToken cancellationToken);
        public Task DeleteFileFromPublicMessageAsync(Guid chatId, Guid attachmentId, Guid deletingBy, CancellationToken cancellationToken);
        public Task DeletePersonalMessageAsync(Guid chatId, Guid messageId, Guid deletingBy, CancellationToken cancellationToken);
        public Task DeleteFileFromPersonalMessageAsync(Guid chatId, Guid attachmentId, Guid deletingBy, CancellationToken cancellationToken);
        public Task SaveMessageAsync(EnChatType chatType, Guid chatId, MessageInput message, CancellationToken cancellationToken);
        public Task<int> UpdateMessageAsync(EnChatType chatType, Guid chatId, Guid messageId, string? newMessageText, DateTime updatedAt, CancellationToken cancellationToken);
        
    }
}
