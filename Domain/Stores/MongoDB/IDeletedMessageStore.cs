using Domain.Models.Documents;

namespace Domain.Stores.MongoDB
{
    public interface IDeletedMessageStore
    {
        public Task<DeletedMessage?> GetOneEldestAsync(CancellationToken cancellationToken);
        public Task<bool> CheckDeletionByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<bool> CreateAsync(DeletedMessage deletedMessage, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
