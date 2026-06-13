using Domain.Models.Documents;

namespace Domain.Stores.MongoDB
{
    public interface IDeletedAttachmentStore
    {
        public Task<DeletedAttachment?> GetOneEldestAsync(CancellationToken cancellationToken);
        public Task<bool> CheckDeletionByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<bool> CreateAsync(DeletedAttachment deletedAttachment, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
