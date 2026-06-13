using Infrastructure.Database;
using Npgsql;

namespace Persistence
{
    public sealed class UnitOfWorkConnectionScope : IAsyncDisposable
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private NpgsqlConnection? _connection;
        private NpgsqlTransaction? _transaction;

        public UnitOfWorkConnectionScope(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public bool HasActiveTransaction => _transaction is not null;

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction is not null)
                throw new InvalidOperationException("Транзакция уже начата.");

            _connection = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
            _transaction = await _connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction is null)
                throw new InvalidOperationException("Транзакция не начата.");

            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            await DisposeTransactionResourcesAsync().ConfigureAwait(false);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction is null)
                throw new InvalidOperationException("Транзакция не начата.");

            await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            await DisposeTransactionResourcesAsync().ConfigureAwait(false);
        }

        public async Task<ConnectionLease> LeaseConnectionAsync(CancellationToken cancellationToken)
        {
            if (_connection is not null)
                return new ConnectionLease(_connection, _transaction, ownsConnection: false);

            var connection = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
            return new ConnectionLease(connection, null, ownsConnection: true);
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction is not null)
                await _transaction.RollbackAsync().ConfigureAwait(false);

            await DisposeTransactionResourcesAsync().ConfigureAwait(false);
        }

        private async Task DisposeTransactionResourcesAsync()
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync().ConfigureAwait(false);
                _transaction = null;
            }

            if (_connection is not null)
            {
                await _connection.DisposeAsync().ConfigureAwait(false);
                _connection = null;
            }
        }
    }

    public readonly struct ConnectionLease : IAsyncDisposable
    {
        public ConnectionLease(NpgsqlConnection connection, NpgsqlTransaction? transaction, bool ownsConnection)
        {
            Connection = connection;
            Transaction = transaction;
            OwnsConnection = ownsConnection;
        }

        public NpgsqlConnection Connection { get; }
        public NpgsqlTransaction? Transaction { get; }
        private bool OwnsConnection { get; }

        public ValueTask DisposeAsync() =>
            OwnsConnection ? Connection.DisposeAsync() : ValueTask.CompletedTask;
    }
}
