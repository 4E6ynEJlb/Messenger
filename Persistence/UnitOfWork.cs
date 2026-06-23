using Domain.Stores;

namespace Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UnitOfWorkConnectionScope _connectionScope;

        public UnitOfWork(
            UnitOfWorkConnectionScope connectionScope,
            ISynchronizationStore synchronization,
            IMaintenanceStore maintenance,
            IGenericChatStore genericChat)
        {
            _connectionScope = connectionScope;
            Synchronization = synchronization;
            Maintenance = maintenance;
            GenericChat = genericChat;
        }

        public ISynchronizationStore Synchronization { get; }
        public IMaintenanceStore Maintenance { get; }
        public IGenericChatStore GenericChat { get; }

        public Task BeginTransactionAsync(CancellationToken cancellationToken = default) =>
            _connectionScope.BeginTransactionAsync(cancellationToken);

        public Task CommitTransactionAsync(CancellationToken cancellationToken = default) =>
            _connectionScope.CommitTransactionAsync(cancellationToken);

        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default) =>
            _connectionScope.RollbackTransactionAsync(cancellationToken);

        public ValueTask DisposeAsync() => _connectionScope.DisposeAsync();
    }
}
