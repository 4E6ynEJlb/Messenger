using Domain.Models.Documents;

namespace Domain.Stores.MongoDB
{
    public interface IMessageUpdateStore
    {
        public Task<MessageUpdate?> GetOneEldestAsync(CancellationToken cancellationToken);
        public Task<bool> CreateAsync(MessageUpdate messageUpdate, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
