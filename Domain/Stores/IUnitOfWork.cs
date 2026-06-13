namespace Domain.Stores
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        public ISynchronizationStore Synchronization { get; }
        public IMaintenanceStore Maintenance { get; }
        public Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        public Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
