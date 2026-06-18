namespace Infrastructure.Storage
{
    public interface IObjectStorage
    {
        public Task<MemoryStream> GetAsync(Guid name, CancellationToken cancellationToken);
        public Task SaveAsync(Stream stream, Guid name, CancellationToken cancellationToken);
        public Task DeleteAsync(Guid name, CancellationToken cancellationToken);
        public Task DeleteManyAsync(IEnumerable<Guid> names, CancellationToken cancellationToken);
    }
}
