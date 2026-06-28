using Domain.Models.Documents;
using Domain.Models.Documents.Keys;

namespace Domain.Stores.MongoDB
{
    public interface IMessageUpdateStore
    {
        public Task<PublicMessageUpdate?> GetOneEldestAsync(CancellationToken cancellationToken);
        public Task<bool> CreateAsync(PublicMessageUpdate messageUpdate, CancellationToken cancellationToken);
        public Task DeleteByChatAsync(Guid chat, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
