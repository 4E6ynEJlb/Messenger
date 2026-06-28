using Domain.Models.Documents;

namespace Domain.Stores.MongoDB
{
    public interface INewMediaStore
    {
        public Task<NewMedia?> GetOneByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<List<NewMedia>> GetManyByIdAsync(List<Guid> id, CancellationToken cancellationToken);
        public Task<bool> CreateAsync(NewMedia newMedia, CancellationToken cancellationToken);
        public Task IncrementMediaLinksAsync(List<Guid> id, CancellationToken cancellationToken);
        public Task DecrementLinksAsync(Dictionary<Guid, int> decrements, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
        public Task DeleteManyAsync(List<Guid> id, CancellationToken cancellationToken);
    }
}
