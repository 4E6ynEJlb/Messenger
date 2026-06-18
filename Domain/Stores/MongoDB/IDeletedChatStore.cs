using Domain.Models.Documents;
using Domain.Models.Types;

namespace Domain.Stores.MongoDB
{
    public interface IDeletedChatStore
    {
        public Task<DeletedChat?> GetOneEldestAsync(CancellationToken cancellationToken);
        public Task SaveAsync(Guid chatId, EnChatType chatType, CancellationToken cancellationToken);
        public Task DeleteAsync(Guid chatId, EnChatType chatType, CancellationToken cancellationToken);
    }
}
