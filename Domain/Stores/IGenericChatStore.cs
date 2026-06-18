using Domain.Models.Types;

namespace Domain.Stores
{
    public interface IGenericChatStore
    {
        public Task<bool> CheckAccessToMessageAsync(EnChatType chatType, Guid chatId, Guid userId, Guid messageId, CancellationToken cancellationToken);
        public Task<bool> CheckAccessToAttachmentAsync(EnChatType chatType, Guid chatId, Guid messageId, Guid attachmentId, CancellationToken cancellationToken);
        public Task<Message[]> GetMessagesByIdAsync(Guid chatId, Guid[] messagesId, Guid gettingBy, EnChatType chatType, CancellationToken cancellationToken);
        public Task PrepareAttachmentsForResending(Guid chatId, Guid[] messagesId, Guid resendBy, EnChatType chatType, CancellationToken cancellationToken);
    }
}
