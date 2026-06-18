using Domain.Models.Documents;
using Domain.Models.Documents.Keys;
using Domain.Models.Types;

namespace Domain.Stores.MongoDB
{
    public interface IDeletedAttachmentStore
    {
        public Task<DeletedAttachment?> GetOneEldestAsync(CancellationToken cancellationToken);
        public Task<List<Guid>> GetDeletedByMessageIdAsync(Guid messageId, EnChatType chatType, Guid chatId, CancellationToken cancellationToken);
        public Task<bool> CreateAsync(DeletedAttachment deletedAttachment, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(DeletedAttachmentKey key, CancellationToken cancellationToken);
    }
}
