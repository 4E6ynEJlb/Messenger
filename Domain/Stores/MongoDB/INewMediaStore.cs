using Domain.Models.Documents;

namespace Domain.Stores.MongoDB
{
    public interface INewMediaStore
    {
        public Task<NewMedia?> GetOneByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<bool> CreateAsync(NewMedia newMedia, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
