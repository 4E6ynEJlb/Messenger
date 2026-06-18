using Domain.Models.Documents;
using Domain.Models.Types;

namespace Domain.Stores.MongoDB
{
    public interface IDeletedMessageStore
    {
        public Task<DeletedMessage?> GetOneEldestAsync(CancellationToken cancellationToken);
        public Task<bool> CheckDeletionByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<uint> GetDeletedCountAsync(Guid chatId, EnChatType chatType, DateTime sentBefore, CancellationToken cancellationToken);
        public Task<bool> CreateAsync(DeletedMessage deletedMessage, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
